#!/bin/sh
set -eu

PASS="$(cat /run/secrets/fs_rabbitmq_password)"

mkdir -p /etc/rabbitmq
printf "default_user = fs_user\ndefault_pass = %s\ndefault_vhost = /\n" "$PASS" > /etc/rabbitmq/rabbitmq.conf

exec /usr/local/bin/docker-entrypoint.sh rabbitmq-server