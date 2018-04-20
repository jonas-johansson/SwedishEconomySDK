# Swedish Economy SDK

## Introduction

The Swedish Economy SDK is a set of C# APIs and programs to help you with Swedish economy, especially to calculate taxes.

## Motivation

When I started a company in Sweden I was confronted with a lot of new concepts. I'm no genious but I thought I had a basic grasp of Swedish economy.
But one of the things that struck me was that all these tax concepts that I believed to be quite simple turned out to be littered with intricate details and conditions. It was complicated.

There is no tax rate you can just slap onto your income and go home.

I found some existing code and repositories but none of them got through my validation, they skipped a few steps.
I started reading through the laws, rules, and called Skatteverket (the Swedish Tax Office) many times to figure out how it actually worked.

I now want to share this with you, courtesy of my game company [Neuston](http://www.neuston.io).

## Status

### Income Tax Calculator
The first part I committed is the Income Tax Calculator which, as the name says, lets you calculate the income taxes. It's an interesting trip to step through the code and look at how the different rules apply to your case.

### Correctness
I take no responsibility for correctness, but the common cases should work.
The examples and unit tests have been validated against Skatteverket (the Swedish Tax Office) to be correct.

### Code Readability
The code is not the sexiest beast but if there's enough interest I might give it a makeover.
