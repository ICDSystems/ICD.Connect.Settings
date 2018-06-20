# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Changed
 - Avoid loading SimplSharp assemblies from build directory in Net Standard

## [6.2.0] - 2018-06-19
### Added
 - Added event arg types for interacting with S+ directly

### Changed
 - Skip extracting plugins that are contained within another plugin
 - Warn when loading core without an available xml config

## [6.1.0] - 2018-06-04
### Added
 - Added abstraction and interface for SimplOriginator
 - Made SPlusShims console nodes

## [6.0.0] - 2018-05-24
### Added
 - Added SPlusGlobalEvents framework, for initializing from s+ and future s+ eventing.
 - Added OnSettingsApplied and OnSettingsCleared events to AbstractSPlusOriginatorShim

### Removed
 - Removed element property from settings

## [5.0.0] - 2018-05-09
### Changed
 - Begin standardizing originator log format
 - Backup configs are saved with date in UTC ISO 8601 format
 - Changed S+ shim naming convention

## [4.1.0] - 2018-05-03
### Added
 - Added ip attribute to settings

## [4.0.0] - 2018-04-27
### Changed
 - KrangSettings factory name and originator type now inferred via reflection

## [3.0.0] - 2018-04-23
### Added
 - Added base SPlus interface for originators
 - Adding originator proxy abstractions

### Changed
 - Removing suffix from assembly name
