# MultiAgentsCoPilot  
  
## Overview  
  
**MultiAgentsCoPilot** is a C# application that simulates a multi-agent system. This system consists of three key roles: **Program Manager**, **Software Engineer**, and **Product Owner**. Together, they collaborate to gather requirements, develop a web application, and ensure that the final product meets the specified criteria.  
  
## Features  
  
- **Program Manager**:   
  - Gathers detailed requirements from users.  
  - Creates a comprehensive development plan.  
    
- **Software Engineer**:   
  - Develops a web application using HTML and JavaScript based on the requirements.  
    
- **Product Owner**:   
  - Reviews the developed application to ensure it meets all requirements.  
  - Approves the final product.  
  
## Prerequisites  
  
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- Azure OpenAI Service credentials  
  
## Configuration  
  
1. Create an `appsettings.json` file in the root directory with the following structure:  
  
   ```json  
   {  
       "AzureOpenAI": {  
           "DeploymentName": "your-deployment-name",  
           "Endpoint": "your-endpoint",  
           "ApiKey": "your-api-key"  
       }  
   }  
