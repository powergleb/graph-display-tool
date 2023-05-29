CREATE DATABASE AdventureWorks;
GO

USE AdventureWorks;
GO
CREATE TABLE user_types (
    id INT PRIMARY KEY,
    type VARCHAR(255)
);


CREATE TABLE users (
    id INT PRIMARY KEY,
    user_type_id INT REFERENCES user_types(id),
    parse_id VARCHAR(255),
    email VARCHAR(255),
    password VARCHAR(255),
    logged_in BIT,
    token_facebook VARCHAR(255),
    token_twitter VARCHAR(255),
    user_token VARCHAR(255),
    token_expiration DATETIME
);

CREATE TABLE user_schedules (
    id INT PRIMARY KEY,
    user_id INT REFERENCES users(id),
    goal VARCHAR(255),
    schedule_cloud_id INT
);


CREATE TABLE user_addresses (
    id INT PRIMARY KEY,
    user_id INT REFERENCES users(id),
    title VARCHAR(255)
);
