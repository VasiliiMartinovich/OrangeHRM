@allure.epic:OrangeHRM
@allure.feature:Authentication
Feature: Authentication
As an OrangeHRM user
I want to log in
So that I can access the application

@allure.story:SuccessfulLogin
@allure.severity:critical
Scenario: Valid credentials open Dashboard
    Given the OrangeHRM login page is displayed
    When I log in with valid credentials
    Then the Dashboard page should be displayed

@allure.story:InvalidLogin
@allure.severity:normal
Scenario: Invalid credentials are rejected
    Given the OrangeHRM login page is displayed
    When I log in with invalid credentials
    Then the login error message should be "Invalid credentials"