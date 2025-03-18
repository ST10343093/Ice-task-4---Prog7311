# 🏨 Hotel Booking System

## 📌 Project Overview
This **ASP.NET Core MVC** application is a **comprehensive hotel booking system** featuring:  
- **Role-based access control** (Admin & Guest)
- **Secure authentication & session management**
- **Dynamic room management** with real-time availability tracking
- **Efficient booking system** with automated pricing & status updates  
It provides an intuitive and responsive interface for both **hotel administrators** and **guests**.

---

## 🏗️ Architecture
The application follows the **Model-View-Controller (MVC)** pattern:

- **Models**: Defines the data structure and business logic  
- **Views**: Manages presentation and user interface  
- **Controllers**: Handles user interactions and application flow  

---

## ✨ Key Features

### 🔑 User Management
#### 🛡️ Authentication System
- Secure **login and registration**
- **Role-based access control** (Admin & Guest)
- **Session management** with remember-me functionality

#### 👥 User Roles
- **Admin**: Full system management  
- **Guest**: Self-service booking  

---

### 🏠 Room Management
#### 📌 Room Inventory
- **Multiple room types** (Single, Double, Suite)  
- Detailed room information (**rates, availability, amenities**)  
- **Real-time availability tracking**  

#### ⚙️ Admin Controls
- **Create, update, and delete** room records  
- **Manage room details & pricing**  
- **Monitor room occupancy**  

---

### 📆 Booking Management
#### 🏷️ Reservation System
- **User-friendly** booking process  
- **Date-range selection** with validation  
- **Dynamic price calculation**  
- **Automatic availability updates**  

#### 📊 Booking Administration
- **Dashboard for admins** to track reservations  
- Booking status management (**Upcoming, Active, Completed, Cancelled**)  
- Guest information tracking  

---

### 💼 Guest Experience
#### 📌 Self-Service Portal
- Browse available accommodations  
- **Make new reservations**  
- **View booking history**  
- **Generate booking receipts**  
- **Cancel upcoming bookings**  

---

## ⚙️ Technical Implementation

### 🗄️ Data Layer
- **Entity Framework Core** (Code-First)  
- **SQL Server** database integration  
- **Database migration support**  

### 🛠️ Data Models
| Model    | Description |
|----------|------------|
| **Room** | Stores room details (number, type, price, availability) |
| **Guest** | Manages guest profiles & contact information |
| **Booking** | Handles reservation details, dates, and status |
| **User** | Maintains authentication details & roles |

### 📌 Booking Flow
1. **Guest selects a room & booking dates**  
2. **System validates availability & calculates price**  
3. **Guest confirms reservation**  
4. **System updates room availability & stores booking details**  

### ✅ Validation
- **Form validation** for all inputs  
- **Business rule enforcement** (e.g., valid dates, room availability)  

---

## 🎨 User Interface

### 📱 Responsive Design
- Built with **Bootstrap** for **mobile & desktop compatibility**  
- **Interactive UI elements** for smooth user experience  

### 🎭 Role-Specific UI
- **Admin dashboard** for managing rooms & bookings  
- **Guest portal** for making and managing reservations  

---

## 🔒 Security Features

### 🔑 Authentication
- **Cookie-based authentication**  
- **Secure password hashing**  
- **Anti-forgery tokens** for form protection  

### 🛡️ Authorization
- **Role-based access control (RBAC)**  
- **Permission-based resource protection**  

---

## 🛠️ System Components

### 🚀 Controllers
| Controller        | Responsibility |
|------------------|---------------|
| **AccountController** | Manages user authentication (Login/Register) |
| **HomeController** | Handles main pages & room discovery |
| **RoomsController** | Admin functionality for managing rooms |
| **GuestsController** | Manages guest profiles & data |
| **BookingsController** | Processes booking, cancellation & receipts |

### 📄 Key Views
| View            | Description |
|----------------|-------------|
| **Home/Index** | Main landing page with system features |
| **Home/AvailableRooms** | Shows available rooms |
| **Account/Login & Register** | User authentication pages |
| **Bookings/Index** | Guest booking dashboard |
| **Bookings/Receipt** | Printable confirmation page |
| **Bookings/Cancel** | Booking cancellation interface |

---

## 🔄 Data Flow

### 🏢 Room Creation
1. **Admin** adds room details (type, price, availability).  
2. Room is **added to the inventory** and displayed on the site.  

### 📅 Booking Process
1. Guest **browses available rooms**.  
2. Selects room & **chooses check-in/check-out dates**.  
3. System **validates availability & calculates the total price**.  
4. Guest **confirms & system stores reservation**.  

### 🏷️ Booking Management
- **Guests view active bookings**.  
- Generate **receipts or cancel reservations**.  
- System **updates availability** upon cancellation.  

---

## 💻 Technologies Used

### 🏢 Backend
- **ASP.NET Core MVC**  
- **Entity Framework Core**  
- **C#**  
- **SQL Server**  

### 🎨 Frontend
- **HTML5, CSS3, JavaScript**  
- **Bootstrap** for responsive UI  
- **jQuery** for interactivity  

### 🛠️ Development Tools
- **Visual Studio**  
- **Git** (Version Control)  
- **NuGet** for package management  

---

## 🚀 Future Enhancements
✔ **Payment gateway integration** for online payments  
✔ **Email notifications** for booking confirmations & reminders  
✔ **Advanced reporting** for admins  
✔ **Room service & amenity booking**  
✔ **Customer reviews & feedback system**  

---

## 📌 Conclusion
This **Hotel Booking System** applies **MVC architecture principles** to create a **robust, secure, and user-friendly** platform for managing hotel operations. It streamlines room management, booking processes, and enhances the guest experience while providing admins with the necessary tools for smooth hotel management.  

---

📌 **Developed using ASP.NET Core MVC, SQL Server, and modern UI frameworks.** 🚀
