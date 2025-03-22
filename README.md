# Event Ease Booking Web App

## 📖 Overview
The **Event Booking Web App** is a modern platform designed to streamline event management. Whether you're hosting a small gathering or a large conference, this app provides users with an intuitive interface to browse, book, and manage events effortlessly.

---

## 🚀 Features
- **Event Listings**: Display detailed event information, including descriptions, dates.
- **Venue Listings**: Display detailed venue information, including descriptions, dates, venues.
- **Event Management**: Tools for organizers to create, edit, and delete events.
- **Dashboard**: Personalized user dashboards to view booked events and manage bookings.

---

## 🛠️ Technologies Used
- **Frontend**: Bootstrap, HTML and CSS
- **Backend**: ASP.NET Core MVC for a robust server-side framework.
- **Database**: SQL Server using Entity Framework Core for data storage and ORM.
- **Cloud Hosting**: Azure App Services for deployment and scalability.

---

## 📂 Installation and Setup
Ensure you have the following installed:
- [.NET 8](https://dotnet.microsoft.com/download)
- A text editor like [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- Access to the database connection details(SQL Server)

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/event-booking-web-app.git

2. Ensure you have the following Nuget Packages Installed
   - Microsoft.EntityFrameworkCore.SqlServer   v9.0.3
   - Microsoft.EntityFrameworkCore.Tools       v9.0.3

3. Replace the connection string in the appsettings.json with your own connection string 

4. Run "Update-Database" in the package manager console