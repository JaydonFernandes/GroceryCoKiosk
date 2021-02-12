# GroceryCoKiosk

GroceryCo. Kiosk is a prototype for a checkout system for customers to use in GroceryCo's supermarket stores. Checkouts are based on price and promotions defined by GroceryCo.

## Usage
The program takes in the full path to a text file which represents the customer's order. The text file should contain an unsorted list of items where each line is a product from the customer's basket scanned at the kiosk. 

An example input file could look like this;

```html
Apple
Orange
Orange
Apple
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

- **name**: Name of the product. (Must not be a null or empty string). 
- **price**: Regular price of the product. (Must be a positive decimal).
- **discount**: Amount to be discounted from the regular price. (Most be a positive decimal and no greater than the product's price).


## Design
For this developer activity I've decided to use techniques I've learned in previous work experience that I believe would be make a program easy to extend, maintain and be production ready.

#### Model View Controller Pattern
Separated the application functionality using the Model, View, Controller (MVC) software architectural design pattern. This promotes organized programing since it clearly defines what each section of the code is responsible for. For the purposes of this project, I used folders to contain each section of the MVC.  

#### Dependency Injection
Used the built in C# dependency injection to create a factory that injects the decencies of an object as needed. This keeps the code modular  and easy to test because it allows classes not to be tightly coupled with classes. Instead, it now depends on an interface which acts as a contract between the class and its dependency.

#### Environment Based Configuration File
Employs an **appsettings.json** that is pulled based on the environment that the application is running. The appsettings.json has configurations for the location for the product catalog JSON file as well as configurations for the logger. This makes the code easier to maintain since any settings for each environment can be configured by using one file.

#### Logger
Added a Logger using [Serilog](https://serilog.net/) to the application that logs events of the application to the text file. The logger logs information events such an item being processed and when a transaction is complete. As well as warning and error events when the application is used in an unexpected way. This benefits the application since the logs can be used in troubleshooting and give insight into the application's usage.

#### Automated Unit Testing with Mocks
Utilizes automated unit testing for business logic. These tests use the [Moq](https://github.com/Moq/moq4/wiki/Quickstart) framework to create mocks such that each each test can focus on a single unit of code without having to manage external dependencies. Automated testing has the advantage of making the code easier to maintain since anytime a change is made, the automated tests can be run to ensure the program will still function as expected. These tests can also be used as a final check before deploying the code to different environments.
