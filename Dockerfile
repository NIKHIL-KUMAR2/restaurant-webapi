FROM python:3.10-slim

# Disable interactive prompts
ENV DEBIAN_FRONTEND=noninteractive

# Install required packages
RUN apt-get update && \
    apt-get install -y --no-install-recommends nano dos2unix && \
    pip install --no-cache-dir ansible pywinrm && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Copy ansible playbooks to image
COPY ./ansible/ /home/ansible/

# Fix line endings and make deploy.sh executable
RUN dos2unix /home/ansible/deploy.sh && chmod +x /home/ansible/deploy.sh

# Set entrypoint to execute the deployment script
ENTRYPOINT ["/home/ansible/deploy.sh"]
