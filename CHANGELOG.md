# Changelog

## 0.7.1 (2024-08-05)

- Improved the internal debug messages.
- Removed unnecessary condition for sending data request. 

## 0.7.0 (2024-07-05)

- Improved the handling of empty and mangled data frames that were causing IndexOutOfRangeException errors.
- Added the ability to log the internal debug messages from Wmr100DataFrameAssembler to an external logger.
- Code refactoring and optimizations.

## 0.6.1 (2024-05-26)

- Added null check to prevent NullReferenceException in Wmr100Device when the Log action is not set.

## 0.6.0 (2024-04-15)

- First public version. No release history before this.