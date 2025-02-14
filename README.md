# eCommerce Microservice Architecture

This project implements a scalable and resilient eCommerce platform using a microservice architecture. The system is designed to handle various business functions such as product management, order processing, and user authentication through independent microservices. An API Gateway serves as the single entry point for client requests, ensuring efficient routing, security, and reliability.

## Key Features

- **API Gateway**: centralized entry point for managing and routing client requests to appropriate microservices.
- **Product API**: handles product-related operations such as listing, searching, and managing product details.
- **Order API**: manages order creation, tracking, and processing.
- **Authentication API**: provides user authentication and authorization services.
- **Rate Limiting**: controls the number of requests a client can make to prevent abuse.
- **Caching**: improves performance by storing frequently accessed data.
- **Retry Mechanism**: ensures reliability by automatically retrying failed requests due to transient errors.

##

![Microservices Diagram](https://i.ibb.co/hJY3Xjd8/Microservices.png)
