After you copy the files, run:

```
cd RabbitMQ

chmod +x init.sh

chmod 600 secrets/*.txt

docker compose -f docker-compose.rabbitmq.yml up -d

```
