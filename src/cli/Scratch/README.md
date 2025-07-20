# Scratch

This console app demonstrates an unawaited asynchronous method call.

## Expected output

Here's what you expect to happen. You hold your breath for three seconds.

10:00:00 Simon says: I have awaken.
10:00:00 Simon says: I am starting an important task. Your computer belongs to me.
10:00:00 Simon says: Hold your breath until my work is complete.
10:00:03 Simon says: I completed my work. Your computer is returned to you.
10:00:03 Simon says: You may breathe again.
10:00:03 Simon says: I am returning to sleep.

## Actual output

Here's what actually happens. You hold your breath forever.

10:00:00 Simon says: I have awaken.
10:00:00 Simon says: I am starting an important task. Your computer belongs to me.
10:00:00 Simon says: Hold your breath until my work is complete.
10:00:00 Simon says: I am returning to sleep.

## An easy fix

The `await` keyword on line 3 is the only thing missing. The bug fixed like this:

```csharp
await simon.Work();
```
