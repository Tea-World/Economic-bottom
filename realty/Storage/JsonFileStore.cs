using System.Text.Json;

namespace ServiceDesk.Api.Storage
{
    /// <summary>
    /// Универсальное хранилище для списка объектов в JSON-файле.
    /// Потокобезопасно, атомарно записывает данные, создаёт файл при отсутствии.
    /// </summary>
    public sealed class JsonFileStore<T> where T : class
    {
        private static readonly Dictionary<string, SemaphoreSlim> _locks = new();
        private static readonly object _locksGuard = new();

        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonFileStore(string filePath, JsonSerializerOptions? jsonOptions = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath is required", nameof(filePath));

            _filePath = Path.GetFullPath(filePath);

            _jsonOptions = jsonOptions ?? new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        /// <summary>
        /// Считать полный список из файла. Если файл отсутствует/пустой/повреждён — вернёт пустой список.
        /// </summary>
        public async Task<List<T>> ReadAllAsync(CancellationToken ct = default)
        {
            var sem = GetFileSemaphore(_filePath);
            await sem.WaitAsync(ct);
            try
            {
                await EnsureFileExistsAsync(ct);

                var json = await File.ReadAllTextAsync(_filePath, ct);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<T>();

                try
                {
                    var data = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
                    return data ?? new List<T>();
                }
                catch (JsonException)
                {
                    // На учебном проекте целесообразно не падать.
                    // В production лучше логировать и/или поднимать ошибку.
                    return new List<T>();
                }
            }
            finally
            {
                sem.Release();
            }
        }

        /// <summary>
        /// Полностью заменить содержимое файла переданным списком (атомарная запись).
        /// </summary>
        public async Task WriteAllAsync(List<T> items, CancellationToken ct = default)
        {
            items ??= new List<T>();

            var sem = GetFileSemaphore(_filePath);
            await sem.WaitAsync(ct);
            try
            {
                await EnsureDirectoryExistsAsync(ct);

                var json = JsonSerializer.Serialize(items, _jsonOptions);

                var dir = Path.GetDirectoryName(_filePath)!;
                var tmp = Path.Combine(dir, $"{Path.GetFileName(_filePath)}.{Guid.NewGuid():N}.tmp");

                await File.WriteAllTextAsync(tmp, json, ct);

                ReplaceFile(tmp, _filePath);
            }
            finally
            {
                sem.Release();
            }
        }

        /// <summary>
        /// Выполнить атомарное обновление: прочитать -> изменить -> записать.
        /// </summary>
        public async Task<TResult> UpdateAsync<TResult>(
            Func<List<T>, TResult> updater,
            CancellationToken ct = default)
        {
            if (updater is null) throw new ArgumentNullException(nameof(updater));

            var sem = GetFileSemaphore(_filePath);
            await sem.WaitAsync(ct);
            try
            {
                await EnsureFileExistsAsync(ct);

                var json = await File.ReadAllTextAsync(_filePath, ct);
                List<T> items;
                try
                {
                    items = string.IsNullOrWhiteSpace(json)
                        ? new List<T>()
                        : (JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>());
                }
                catch (JsonException)
                {
                    items = new List<T>();
                }

                var result = updater(items);

                var newJson = JsonSerializer.Serialize(items, _jsonOptions);

                var dir = Path.GetDirectoryName(_filePath)!;
                var tmp = Path.Combine(dir, $"{Path.GetFileName(_filePath)}.{Guid.NewGuid():N}.tmp");
                await File.WriteAllTextAsync(tmp, newJson, ct);

                ReplaceFile(tmp, _filePath);

                return result;
            }
            finally
            {
                sem.Release();
            }
        }

        private static SemaphoreSlim GetFileSemaphore(string filePath)
        {
            lock (_locksGuard)
            {
                if (!_locks.TryGetValue(filePath, out var sem))
                {
                    sem = new SemaphoreSlim(1, 1);
                    _locks[filePath] = sem;
                }
                return sem;
            }
        }

        private async Task EnsureFileExistsAsync(CancellationToken ct)
        {
            await EnsureDirectoryExistsAsync(ct);

            if (!File.Exists(_filePath))
            {
                // создаём пустой список
                await File.WriteAllTextAsync(_filePath, "[]", ct);
            }
        }

        private Task EnsureDirectoryExistsAsync(CancellationToken ct)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (string.IsNullOrWhiteSpace(dir))
                throw new InvalidOperationException($"Invalid directory for path: {_filePath}");

            Directory.CreateDirectory(dir);
            return Task.CompletedTask;
        }

        private static void ReplaceFile(string tmp, string target)
        {
            // На Windows File.Replace даёт атомарную замену.
            // На других ОС используем Move с overwrite (в .NET 6+ доступно).
            try
            {
                if (File.Exists(target))
                {
                    // backup не нужен в учебном проекте
                    File.Replace(tmp, target, destinationBackupFileName: null, ignoreMetadataErrors: true);
                }
                else
                {
                    File.Move(tmp, target);
                }
            }
            catch
            {
                // fallback
                File.Move(tmp, target, overwrite: true);
            }
        }
    }
}
