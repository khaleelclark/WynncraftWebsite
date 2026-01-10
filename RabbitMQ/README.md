# RabbitMQ Secrets

Copy the `.example.txt` files to `.txt` and replace values.

These files are mounted as Docker secrets.
Do NOT commit real passwords.

After you copy the files, run:

```
cd RabbitMQ

chmod +x init.sh

chmod 600 secrets/*.txt

docker compose -f docker-compose.rabbitmq.yml up -d

```
