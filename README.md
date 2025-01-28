# OpenApi
Вызывать из папки DeliveryApp.Api/Adapters/Http/Contract
```
cd DeliveryApp.Api/Adapters/Http/Contract/
openapi-generator generate -i https://gitlab.com/microarch-ru/microservices/dotnet/system-design/-/raw/main/services/delivery/contracts/openapi.yml -g aspnetcore -o . --package-name OpenApi --additional-properties classModifier=abstract --additional-properties operationResultTask=true
```
# БД
```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
```
[Подробнее про dotnet cli](https://learn.microsoft.com/ru-ru/ef/core/cli/dotnet)

# Миграции
```
dotnet ef migrations add Init --startup-project ./DeliveryApp.Api --project ./DeliveryApp.Infrastructure --output-dir ./Adapters/Postgres/Migrations
dotnet ef database update --startup-project ./DeliveryApp.Api --connection "Server=localhost;Port=5432;User Id=username;Password=secret;Database=delivery;"
```

# Запросы к БД
```
-- Выборки
SELECT * FROM public.couriers;
SELECT * FROM public.courier_statuses;
SELECT * FROM public.transports;

SELECT * FROM public.orders as o
LEFT JOIN public.order_statuses as s on o.status_id=s.id;

SELECT * FROM public.order_statuses;
SELECT * FROM public.outbox;

-- Очистка БД (все кроме справочников)
DELETE FROM public.couriers;
DELETE FROM public.orders;
DELETE FROM public.outbox;

-- Добавить курьеров
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('bf79a004-56d7-4e5f-a21c-0a9e5e08d10d', 'Пеший 1', 1, 1, 3, 2);
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('a9f7e4aa-becc-40ff-b691-f063c5d04015', 'Пеший 2', 1, 3,2, 2);    
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('db18375d-59a7-49d1-bd96-a1738adcee93', 'Вело 1', 2, 4,5, 2);
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('e7c84de4-3261-476a-9481-fb6be211de75', 'Вело 2', 2, 1,8, 2);    
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('407f68be-5adf-4e72-81bc-b1d8e9574cf8', 'Авто 1', 3, 7,9, 2);
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('006e6c66-087e-4a27-aa59-3c0a2bc945c5', 'Авто 1', 3, 5,5, 2);      
```