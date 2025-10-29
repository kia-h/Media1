# FunctionalTest Project

This project is intended for **functional / end-to-end testing** of the Media1 application.

---

## Current Status

- SpecFlow (BDD-style testing) is **not fully supported in .NET 8** at this time. use ReqnRoll
- This project demonstrates awareness of behavior-driven testing concepts and how they would be applied once supported.
- following will be the scenarios, then you need to have code behind in c# for each step
---

## Possible Example Scenario (BDD)


Feature: Video Upload and Catalogue
  As a user
  I want to upload videos and see them listed
  So that I can play them in the catalogue

  Scenario: Successfully upload a supported video
    Given I have a video file "sample.mp4"
    When I upload the video
    Then the video should appear in the catalogue

  Scenario: Upload a file with disallowed extension
    Given I have a video file "sample.txt"
    When I upload the video
    Then I should receive an error "File type txt is not allowed"

  Scenario: Upload a file exceeding max size
    Given I have a video file "bigvideo.mp4" of size 250MB
    When I upload the video
    Then I should receive an error "File {FileName} exceeds the maximum allowed size of 200mb"
