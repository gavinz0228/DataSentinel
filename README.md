# DataSentinel
A generic back end designed for CRUD operations on local database for web scraping.

## Set Up

**Database:**

Set up a MongoDB instance, and create a database and its collections before this api can be used.

**Environment Variables:**

1. JWT Token secret key, default key: TOKEN_SECRET_KEY
2. Login user name and password, default key: USER_NAME, PASSWORD

(These Environment variable keys can be changed in the appsettings.json under section Constants.)

## Usage 

1. Send a post request with user name and password to api/auth/login to get the jwt token.
2. Call rest api with JWT token in the request header:  

        Authorization: Bearer <token>

example of a Get request:
http://localhost:5000/api/data/get/user/{"FirstName":"John"}
