# DTE Event Handlers

DTE Event Handlers is a project designed to handle scheduled tasks and provide custom Cognito message handling. The project is built with ASP.NET Core and provides functionality required to efficiently manage event-driven jobs.

## Table of Contents
- [Project Description](#project-description)
- [How to Install and Run the Project](#how-to-install-and-run-the-project)
- [Usage](#usage)

## Project Description
DTE Event Handlers is a comprehensive system designed to handle specific scheduled tasks within the project ecosystem. It provides a custom Cognito message handler that allows for specific manipulation and management of Cognito-based messages. 

The scheduled jobs are designed to run at specified intervals, performing various tasks essential to the smooth functioning of the system. The custom Cognito message handler adds a layer of customization to the standard Cognito messages, allowing them to be tailored to specific project requirements.

## How to Install and Run the Project
To run the project, you will need to have the following installed on your machine:

- .NET Core 6 SDK or later

To run the project, follow these steps:
1. Clone the repository to your local machine.
2. Update `appsettings.json` with the appropriate settings.
3. Right click on the DTE Event Handlers project and select properties. When the modal pops up select run/configurations/default and set the environment variable ASPNETCORE_ENVIRONMENT to Development.
4. Start the project from your chosen IDE or from the command line with `dotnet run`.

## Usage
To use the project, you can follow these steps:

1. Set the environment variable `ASPNETCORE_ENVIRONMENT` to `Development`.
2. Start the project.
3. Interact with the project through your chosen IDE, noting that scheduled jobs will run at their designated intervals and the Cognito message handler will process messages as they arrive.

## Harness
To use the project, you can follow these steps:

To run functions locally, you can use the harness provided in the project. To use the harness, follow these steps:

1. Install [Docker](https://www.docker.com/products/docker-desktop) and [Docker Compose](https://docs.docker.com/compose/install/).
2. Navigate to the Solution Items folder and run docker-compose up.
3. Install [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html).
4. Create local buckets for the project by running the following commands:
    - `aws --endpoint-url=http://localhost:4566 s3api create-bucket --bucket local-odp-export-bucket --region us-east-1`
    - `aws --endpoint-url=http://localhost:4566 s3api create-bucket --bucket local-export-bucket --region us-east-1`
5. Create a local DynamoDB table for the project by running the following command:
    - `aws --endpoint-url=http://localhost:4566 dynamodb create-table \
      --table-name local-participant \
      --attribute-definitions \
      AttributeName=Email,AttributeType=S \
      AttributeName=NhsNumber,AttributeType=S \
      AttributeName=PK,AttributeType=S \
      AttributeName=ParticipantId,AttributeType=S \
      AttributeName=ParticipantRegistrationStatus,AttributeType=N \
      AttributeName=SK,AttributeType=S \
      --key-schema AttributeName=PK,KeyType=HASH AttributeName=SK,KeyType=RANGE \
      --billing-mode PAY_PER_REQUEST \
      --global-secondary-indexes \
      'IndexName=ParticipantRegistrationStatusIndex,KeySchema=[{AttributeName=ParticipantRegistrationStatus,KeyType=HASH},{AttributeName=SK,KeyType=RANGE}],Projection={ProjectionType=ALL}' \
      'IndexName=EmailIndex,KeySchema=[{AttributeName=Email,KeyType=HASH}],Projection={ProjectionType=INCLUDE,NonKeyAttributes=[CreatedAtUtc]}' \
      'IndexName=NhsNumberIndex,KeySchema=[{AttributeName=NhsNumber,KeyType=HASH}],Projection={ProjectionType=INCLUDE,NonKeyAttributes=[CreatedAtUtc]}' \
      'IndexName=StudyStatusIndex,KeySchema=[{AttributeName=PK,KeyType=HASH},{AttributeName=ParticipantRegistrationStatus,KeyType=RANGE}],Projection={ProjectionType=ALL}' \
      'IndexName=StudyParticipantIndex,KeySchema=[{AttributeName=PK,KeyType=HASH},{AttributeName=ParticipantId,KeyType=RANGE}],Projection={ProjectionType=ALL}'
      `
6. Update `appsettings.json` with the appropriate settings.
7. Update aws credentials in `~/.aws/credentials` with the following:
    - `[default]`
    - `aws_access_key_id = test`
    - `aws_secret_access_key = test`
    - `region = us-east-1`
