# Swedish Economy SDK

## Introduction

The Swedish Economy SDK is a set of C# APIs and programs to help you with Swedish economy, especially related to taxation.

## Motivation

When I started a company in Sweden recently I was confronted with a lot of new concepts. I’m no genius but I thought I had a basic grasp of Swedish economy. But one of the things that struck me was that all these tax concepts that I had believed to be quite simple turned out to be littered with intricate details and conditions. It was complicated.

There is no simple formula you can just throw at your calculator to know how much tax you’re supposed to pay.

Since I write software for a living I wanted to create a system of the rules and put them to practice. There were some existing half-solutions out there that unfortunately didn’t pass the tests. So I started reading through the laws, rules, and called Skatteverket (the Swedish Tax Office) to figure out how things actually worked and started working on this SDK.

I now want to share this with you, courtesy of my game company [Neuston](http://www.neuston.io).

## Status

This is the very beginning and I chose to start small to gather feedback. The first released part is the Income Tax Calculator which, as the name says, lets you calculate the income taxes of a single individual. It’s an interesting trip to step through the code and see the mechanics at play.

I take no legal responsibility for correctness, but the common cases should work. The examples and unit tests have been validated against Skatteverket (the Swedish Tax Office) to be correct.

The code is not the sexiest beast but if there’s enough interest I might give it a makeover.

Have fun!
