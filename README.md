# Reddit WebRole Application - Cloud Application Development

## Project Overview

This project implements a Reddit-like discussion forum with multiple services handling different functionalities like user interactions, notifications, and system health monitoring. The system is designed to be robust and scalable, using Azure services and modern web technologies.

## Key Features

### Reddit
- **Home**: A central page where users can view all topics within the application.
- **AddTopic**: Allows users to contribute new topics for discussion.
- **AddComment**: Enables users to add comments to existing topics.
- **EditProfile**: Users can update their account details.
- **LogIn**: Page for user authentication.
- **Register**: Allows new users to create an account.

### HealthStatus
- **Home**: Monitors the status of the Reddit service and notification systems.

### AdminConsole
- **Console**: Administrative tool that provides capabilities such as viewing and deleting users, topics, and comments.

### Notification Service (Worker Role)
- **Email Notifications:** Sends email notifications to subscribed users without interrupting the main application flow, using services like Postmark or SendGrid.
- **Message Queuing:** Utilizes Azure Queue Storage to handle messages efficiently.

### Health Monitoring Service (Worker Role)
- **System Availability Checks:** Regularly checks the health of the RedditService and NotificationService and logs the results.
- **Alerts on Failures:** Sends email alerts if a service check fails.

### Health Status Service (Web Role)
- **Service Uptime Display:** Shows the availability of the RedditService in real-time and displays uptime statistics for the last 24 hours.

## Pages through images
Visual previews of the application pages to provide a better understanding of the user interface.

| Page          | Image Preview |
|---------------|---------------|
| **Home**      | ![Home](https://github.com/dusandulo/cloud-app/blob/main/imgpreviews/pocetna.png) |
| **AddTopic**  | ![AddTopic](https://github.com/dusandulo/cloud-app/blob/main/imgpreviews/new_topic.png) |
| **AddComment**| ![AddComment](https://github.com/dusandulo/cloud-app/blob/main/imgpreviews/add_comment.png) |
| **EditProfile**| ![EditProfile](https://github.com/dusandulo/cloud-app/blob/main/imgpreviews/edit_profile.png) |
| **LogIn**     | ![LogIn](https://github.com/dusandulo/cloud-app/blob/main/imgpreviews/login.png) |
| **Register**  | ![Register](https://github.com/dusandulo/cloud-app/blob/main/imgpreviews/register.png) |
| **HealthStatus** | ![HealthStatus](https://github.com/dusandulo/cloud-app/blob/main/imgpreviews/health_status.png) |
| **AdminConsole** | ![AdminConsole](https://github.com/dusandulo/cloud-app/blob/main/imgpreviews/admin_console.png) |

## Technologies Used
- **Azure Cloud Services:** Utilizes various Azure services including Azure Table Storage, Queue Storage, and Worker Roles.
- **Web Technologies:** MVC architecture for Web Role applications.
- **Email Services:** Integration with third-party email services for reliable notification delivery.

## Getting Started

### Prerequisites
- Azure subscription
- Visual Studio 2019 or later
- .NET Core SDK

### Installation
1. Clone the repository:
git clone https://github.com/dusandulo/cloud-app.git
2. Navigate to the solution file and open it with Visual Studio.
3. Restore NuGet packages:
dotnet restore
4. Configure Azure services according to the `azure-setup.md` file.
5. Run the application from Visual Studio or deploy it to Azure Cloud.

## Contribution Guidelines

Please read [CONTRIBUTING.md](/) for details on our code of conduct, and the process for submitting pull requests to us.

## License

This project is licensed under the MIT License - see the [LICENSE.md](/) file for details.
