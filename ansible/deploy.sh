#!/bin/sh
set -e

INVENTORY="/home/ansible/hosts.ini"

cat > "$INVENTORY" <<EOF
[windows]
$APP_IP

[windows:vars]
ansible_user=Administrator
ansible_password=$APP_PASSWORD
ansible_connection=winrm
ansible_winrm_transport=basic
ansible_winrm_server_cert_validation=ignore
ansible_port=5985
ansible_winrm_read_timeout_sec=120
ansible_winrm_operation_timeout_sec=60
EOF

echo "Starting Ansible deployment..."

ansible-playbook -i "$INVENTORY" /home/ansible/install_iis.yml

ansible-playbook -i "$INVENTORY" /home/ansible/deploy_application.yml

ansible-playbook -i "$INVENTORY" /home/ansible/update_config.yml

ansible-playbook -i "$INVENTORY" /home/ansible/enable_logs.yml

echo "Ansible deployment completed."
