# GroceryCoKiosk

GroceryCo. Kiosk is a kiosk checkout system for customers to use in the their supermarket stores. Checkouts are based on price 



## Usage
The program takes in the full path to a text file which represents the customer's order. The text file should contain an unsorted list of items where each line is a product from the customer's basket scanned at the kiosk. 

An example input file could look like this;

```html
Apple
Banana
Orange
Banana
```

An example output;
```html
++++++++++++++++++++++++++++++++++++++
Friday February 12, 2021        1:39 AM

Orange
        Regular price: $1.25
        Price after discount: $1.05
        Quantity: 2
        Item total: $2.10

Apple
        Regular price: $0.75
        Price after discount: $0.50
        Quantity: 2
        Item total: $1.00

Banana
        Regular price: $1.00
        Quantity: 1
        Item total: $1.00

======================================
Subtotal (5 items):              $5.00
Discount:                       -$0.90
--------------------------------------
Total:                           $4.10
======================================

 Thank you for shopping at GroceryCo.
++++++++++++++++++++++++++++++++++++++
```


The catalog of products can be set in ```GroceryCoKiosk/GroceryCoKiosk/Data/products.json```.
The file format is a list of JSON objects where each valid object has the following fields;

- **name**: Name of the product. (Must not be null or empty string.) 
- **price**: Regular price of the product. (Must be positive decimal.)
- **discount**: Amount to be discounted from the regular price. (Most be positive decimal )


## Design
For this developer activity I've decided to use techniques I've learned in previous work experience that I believe would be make a program easy to extend, maintain and be production ready.


#### Dependency Injection
Used the default C# dependency injection to create a factory that injects the decencies of an object as needed. This keeps the code modular  and easy to test because it allows classes not to be tightly coupled with classes. Instead it now depends on a interface which acts as a contract between the class and its dependency.

#### Environment Based Configuration File
Employs an **appsettings.json** that is pulled based on the environment that the application is running. The appsettings.json has configurations for the location for the product catalog JSON file as well as configurations for the logger.

#### Logger
Added a Logger using [Serilog](https://serilog.net/) to the application that logs events of the application to the text file. The logger logs information events such an item being processed and when a transaction is complete. As well as warning and error events when the application is used in an unexpected way. This would be beneficial to the application since the logs can be used in troubleshooting and give insight into the application's usage.

#### Automated Unit Testing with Mocks
Utilizes automated unit testing for business logic. These tests



