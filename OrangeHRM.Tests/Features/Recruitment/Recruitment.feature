Feature: Recruitment management
As an OrangeHRM recruiter
I want to manage candidates and vacancies
So that I can support the recruitment process

Background:
    Given the OrangeHRM login page is displayed
    When I log in with valid credentials
    Then the Dashboard page should be displayed

@CreateCandidate
@Recruitment @Candidates
Scenario: Create candidate with CV and vacancy
    Given a vacancy is available for hiring
    When I create a candidate with a CV and apply for the vacancy
    Then candidate details should match the submitted information
    And the uploaded CV and selected vacancy should be displayed

@DeleteCandidate
@Recruitment @Candidates
Scenario: Delete an existing candidate
    Given a candidate with a CV and vacancy exists
    When I delete the candidate from the candidates list
    Then the candidate deletion confirmation should be displayed
    And the deleted candidate should no longer appear in search results

@Recruitment @Vacancies
Scenario: Create vacancy with hiring manager
    Given an employee is available as hiring manager
    When I create a vacancy for the job title "Software Engineer"
    Then the saved vacancy should contain the submitted information