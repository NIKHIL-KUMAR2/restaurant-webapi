# Restaurant Manager Web API configuration and CI/CD Pipeline

This repository contains Ansible playbooks and a CI/CD pipeline to deploy a .NET Web API application (`restaurant-manager`) toWindows server running IIS server. It configures the server,automate application deployment and logs setup using Jenkins and Ansible, .

---

## Components

### Ansible Playbooks

#### 1. `install_iis.yml`

Installs IIS and required components on a Windows server.

**Tasks:**

-   Install and ensures IIS base features with management tools are installed.
    
-   Enable necessary IIS features such as ASP.NET, HTTP redirect, tracing, and developer tools.
    
-   Ensure the IIS service (`W3SVC`) is started and enabled.
    

#### 2. `deploy_application.yml`

Deploys the built application files to IIS from Jenkins workspace.

**Tasks:**

-   Ensures default IIS website is removed.
    
-   Create necessary directories for deployment and logging.
    
-   Copy application files to the website root.
    
-   Create and configure an IIS application pool.
    
-   Create a new IIS website pointing to the deployed application.
    
-   Start the website and application pool.
    

#### 3. `update_config.yml`

Updates the application's connection string in the `Web.config` file.

**Tasks:**

-   Set the `connectionString` value for the `MyDbConnection` entry.
    
-   Set the `providerName` to use Npgsql (PostgreSQL .NET provider).
    

#### 4. `enable_logs.yml`

Installs and configures the Amazon CloudWatch Agent on the Windows EC2 instance.

**Tasks:**

-   Download and install the CloudWatch Agent.
    
-   Upload a predefined configuration file.
    
-   Apply the configuration using PowerShell.
    
-   Ensure the CloudWatch Agent service is started and enabled.
    

----------

## Jenkins Pipeline

The `Jenkinsfile` defines the CI/CD pipeline that performs the following tasks:

1.  **Checkout Code** – Retrieves the source code from GitHub.
    
2.  **Restore NuGet Packages** – Installs project dependencies using NuGet.
    
3.  **Build** – Compiles the solution using MSBuild with Release and DeployOnBuild arguments.
    
4.  **Deploy with Ansible** – Builds and runs a Docker container to execute Ansible playbooks.
    

### Notes:

-   Secrets likea IP addresses, EC2 instance credentials and database credentials are fetched from AWS Systems Manager Parameter Store.
    
-   The deployment is executed inside a container built from the provided Dockerfile.
    
----------

## Dockerfile

The Dockerfile creates Anisible execution environment:

-   Based on `python:3.10-slim`.
    
-   Installs Ansible and `pywinrm` for Windows remote management.
    
-   Copies Ansible playbooks into the container.
    
-   Converts line endings and makes the deployment script executable.
    
-   Executes `deploy.sh` on container start.
    

----------

## Shell Script: `deploy.sh`

This script is the entry point for Ansible image.

**Taks:**

-   Dynamically creates an inventory file (`hosts.ini`) using environment variables.
    
-   Executes all Ansible playbooks in sequence:
    
    -   `install_iis.yml`
        
    -   `deploy_application.yml`
        
    -   `update_config.yml`
        
    -   `enable_logs.yml`
        


## Requirements

### EC2 instance
-   Should be Windows server image

-   Accessible via WinRM (port 5985).
    
-   Requires Administrator credentials.
    
-   Should have necessary IAM roles and security group permissions for CloudWatch.
    

### Jenkins Environment

-   Docker must be installed.
    
-   AWS credentials must be set in Jenkins credentials for accessing Parameter Store.
    
-   Git access and build tools (NuGet, MSBuild) should be present to build .Net Framework application.

## Project Structure

```
📦 restaurant-manager-project
├── ansible/
│   ├── install_iis.yml
│   ├── deploy_application.yml
│   ├── update_config.yml
│   ├── enable_logs.yml
│   ├── config.json
│   └── deploy.sh
├── Dockerfile
├── Jenkinsfile
└── README.md
```
