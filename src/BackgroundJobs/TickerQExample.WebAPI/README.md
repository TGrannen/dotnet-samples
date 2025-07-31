# TickerQ

A demonstration project showcasing the implementation of background jobs and scheduled tasks using [TickerQ](https://tickerq.arcenox.com/) in a .NET application.

## Overview

TickerQExample is a sample implementation of background job processing using the TickerQ framework. It demonstrates how to schedule and manage both cron-based recurring tasks and
time-based background jobs in an ASP.NET Core application.

## Features

- Cron-based scheduled tasks
- Time-based background jobs
- Database-backed job persistence
- Retry mechanisms for failed jobs
- Batch job processing capabilities

## Example Background Jobs

The project includes several example background jobs demonstrating different scheduling patterns:

1. **ExampleMethod**
    - Runs every 5 minutes
    - Simple logging demonstration

2. **WithObject**
    - Example of a job with custom object parameters
    - Shows how to pass complex data to background jobs

## Prerequisites

- .NET 9.0

## Getting Started

1. Run `TickerQExample.AspireHost` project

## Project Structure

The project follows a standard ASP.NET Core structure with additional components for background job processing:

- `BackgroundJobs/`: Contains background service implementations
- `Data/`: Includes database migrations and context
