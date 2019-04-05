# DataSentinel
A generic back end designed for CRUD operations on local database for web scraping.

## Set Up

**Database:**

Set up a MongoDB instance, and create a database and its collections before this api can be used.

**Environment Variables:**

1. JWT Token secret key, default key: BACKEND_TOKEN_SECRET_KEY
2. Login user name and password, default key: BACKEND_USER_NAME, BACKEND_PASSWORD
3. MongoDB connection string and database name: BACKEND_DB_CONNECTION, BACKEND_DB_NAME

(These Environment variable keys can be changed in the appsettings.json under section Constants.)

## Usage 

1. Send a post request with user name and password to api/auth/login to get the jwt token.

    - Request: 
        - Post: http://localhost:5000/api/auth/login 
        - Data:{"User Name": "admin", "Password": "password"} 
        - Header: Content-Type -> application/json

    - Response:
        - {
            "userName": "admin",
            "password": null,
            "token": "eyJhbGciOiJIUzI1NiIsInR5caI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImdhdmcuMDIyOCIsIm5iZiI6MTU1MjYwNjEyOCwiZXhwIjoxNTUzMjEwOTI4LCJpYXQiOjEqNTI2MDYxMjh9.N31JZR4vaQRGxpzO8JUUc9TA-jP9v-0lwXAvQ_xDPes"
        }

2. Call rest api with JWT token in the request header:  

        Authorization: Bearer <token>

example of a Get request:
http://localhost:5000/api/data/get/user/{"FirstName":"John"}
