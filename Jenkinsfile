pipeline {
    agent {label 'windows-build-server'}

    environment {
        NUGET = 'C:\\Tools\\nuget\\nuget.exe'
        MSBUILD = '"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe"'
        DB_PORT = 5432
    }

    stages {
        stage('Checkout Code') {
            steps {
                git branch: "${env.BRANCH}", url: 'https://github.com/NIKHIL-KUMAR2/restaurant-webapi.git'
            }
        }

        stage('Prepare') {
            parallel {
                stage('Docker Build Ansible image') {
                    steps {
                        bat '''
                        docker build -t ansible-deployer -f Dockerfile .
                        '''
                    }
                }

                stage('Restore and Build App') {
                    stages {
                        stage('Install NuGet Packages') {
                            steps {
                                bat "${NUGET} restore Restaurant-WebAPI.sln"
                            }
                        }
                        stage('Build application') {
                            steps {
                                bat "${MSBUILD} Restaurant-WebAPI.sln -p:DeployOnBuild=true -p:Configuration=Release"
                            }
                        }
                    }
                }
            }
        }

        stage('Deploy application to server') {
            steps {
                script {
                    // Function to fetch variables from AWS parameter store and create .env file
                    def fetchVarsAndCreateEnv = { envStage ->
                        withAWSParameterStore(
                            credentialsId: 'aws_credential',
                            naming: "relative",
                            path: "/restaurant-manager/${envStage}",
                            recursive: true,
                            regionName: 'ap-south-1'
                        ) {
                            if (!env.APP_IP?.trim() || !env.APP_PASSWORD?.trim() || !env.RDS_ENDPOINT?.trim() || !env.RDS_PASSWORD?.trim()) {
                                error("Missing required environment variables")
                            }

                            writeFile file: '.env', text: """
                            APP_IP=${env.APP_IP}
                            APP_PASSWORD=${env.APP_PASSWORD}
                            DB_HOST=${env.RDS_ENDPOINT}
                            DB_NAME=restaurant
                            DB_USERNAME=${env.RDS_USER}
                            DB_PASSWORD=${env.RDS_USERPASSWORD}
                            DB_PORT=${env.DB_PORT}
                            """
                        }
                    }

                    fetchVarsAndCreateEnv(env.ENV_STAGE)

                    bat '''
                    docker run --rm ^
                    --env-file .env ^
                    -v %WORKSPACE%:/workspace ^
                    ansible-deployer

                    del .env
                    '''
                }
            }
        }
    }
}
