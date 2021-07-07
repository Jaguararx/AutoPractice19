Feature: Main Tests

Scenario: Main page have a title, a header and a description
    Given a driver is created
    When the main page is loaded
    Then the main page have a title 
    And the title is equal to 'COE Test Automation Sample App'
    And the main page have a header 
    And the header is equal to 'COE Test Automation Sample App'
    And the main page have a description
    And the description contains 'Lorem ipsum dolor sit amet'

Scenario Outline: Dropdown test
  When the main page is loaded
  Then list item with index '<index>' has value '<value>'
Examples:
  | index | value       |
  | 0     | list item 1 |
  | 1     | list item 2 |
  