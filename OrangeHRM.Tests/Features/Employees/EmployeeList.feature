Feature: Employee list
As an OrangeHRM user
I want to view and search employees
So that I can find employee information

Background:
    Given the OrangeHRM login page is displayed
    When I log in with valid credentials
    Then the Dashboard page should be displayed

Scenario: Open the employee list
    When I open the PIM employee list
    Then the employee list page should be displayed

@SearchEmployee
Scenario: Search for a non-existing employee
    When I open the PIM employee list
    And I search for non-existing employee "NonExistingEmployee123"
    Then no employees should be found