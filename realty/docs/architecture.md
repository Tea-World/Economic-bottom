# Архитектура InvestCalc

Общий поток работы сервиса

## Архитектура основана на разделении ответственности:

Клиент → REST API → Контроллер → Сервис → Модели → Ответ

## Контейнеры:

Клиент - Клиент

REST API InverCals - для API запросов

CalsController - для ввода значений

LocalCalsService - локальный расчет

CloudCalsService - облачный расчет

SensitivityService

CompareService

CalsRequest - запрос на сервер

CalsResponse -  ответ сервера

