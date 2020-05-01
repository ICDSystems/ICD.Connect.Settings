# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [10.0.0] - 2020-03-20
### Added
 - Added validation methods to settings
 - Permissions are serialized back to XML

### Changed
 - Settings will be validated before being applied. Critical (or worse) errors will prevent settings being applied.
 - Fixed potential null reference exception when loading permissions
 - Changed IdUtils to use enum for Subsystems, and generate DAV-style ID's

## [9.3.0] - 2019-11-20
### Changed
 - Settings backups are now stored in the ProgramXXData directory.

## [9.2.0] - 2019-11-20
### Added
 - Added a MAC Address element for the processor in the configuration header

## [9.1.0] - 2019-09-16
### Added
 - Added originator groups
 - Moved Order and Disable properties and events from Source/Destination to Originator
 - Added localization settings

### Changed
 - Originator logging includes the CombineName

## [9.0.0] - 2019-08-15
### Added
 - SPlus Shim Request Resync with API
 - SPlusSafeString for safely sending strings to S+

### Changed
 - Better logging for cyclic dependency detection

## [8.2.2] - 2019-08-27
### Changed
 - Changed logging to be more accurate for when an assembly fails to be cached.
 - Skipping obj directories when looking for dependencies.

## [8.2.1] - 2019-08-15
### Changed 
 - Failing more gracefully when a duplicate settings factory name is cached.

## [8.2.0] - 2019-06-06
### Added
 - Added v4.0 to v5.0 config migration for converting to new room combine structure
 - Added ConfigUtils class
 
### Changed
 - Moved util classes into Utils directory

## [8.1.3] - 2019-06-21
### Changed
 - Fixed migration issue with URIs gaining additional '?' characters in the query

## [8.1.2] - 2019-05-08
### Changed
 - Fixed migration issues with URI defaults (host, scheme, port and path)
 - Fixed migration issue where Crestron was serializing newlines into empty elements

## [8.1.1] - 2019-04-30
### Changed
 - Slightly faster plugin caching
 - Fixed bug where empty/null URLs would break migration
 - Clarifying migration exceptions

## [8.1.0] - 2019-03-13
### Added
 - Added config migration for ConferencePoints

### Changed
 - Incremented config version from 3.1 to 4.0

## [8.0.0] - 2019-01-10
### Added
 - Added config migration for port configuration
 - Added Log method to AbstractOriginator for exceptions

### Changed
 - Moving originators and settings classes into subdirectories
 - Incremented config version from 3.0 to 3.1

## [7.6.1] - 2020-04-30
### Changed
 - Header CompiledOn is handled as a Date

## [7.6.0] - 2019-08-13
### Changed
 - Marked LicensePath as obsolete in favor of SystemKeyPath

## [7.5.2] - 2019-07-31
### Changed
 - Small performance improvement in plugin discovery
 - Telemetry service is optional

## [7.5.1] - 2019-06-24
### Changed
 - LoadCore will fail if the XML configuration is not valid XML

## [7.5.0] - 2019-05-16
### Added
 - Added telemetry features to originators

## [7.4.1] - 2019-05-24
### Added
 - Added CardAddressSettingsPropertyAttribute
 - Added CardParentSettingsPropertyAttribute
 - Added settings property attribute abstractions and interfaces

## [7.4.0] - 2019-05-16
### Added
 - Added cyclic dependency validation when loading core settings

### Changed
 - Potential fix for various shutdown exceptions, don't clear originator ID on dispose

## [7.3.3] - 2019-05-14
### Changed
 - v2 to v3 config migration now combines multiple destinations into a single destination with multiple addresses

## [7.3.2] - 2019-04-03
### Changed
 - Fixed config migration issue where display volume control id changed between v2 and v3

## [7.3.1] - 2019-03-13
### Changed
 - Fixed version comparison issue that was causing unwanted migrations

## [7.3.0] - 2019-01-02
### Added
 - Added migration features for automatically updating older configs
 - Added migration for config version 2.0 to 3.0
 - Added support for multiple KrangSettings attributes per class

## [7.2.0] - 2018-11-08
### Added
 - Writing document encoding information to saved configurations

### Changed
 - SettingsCollection events raise the item added or removed

## [7.1.0] - 2018-10-30
### Added
 - Added ControlPortParentSettingsPropertyAttribute for better parent device binding on DeployAV

### Changed
 - Fail gracefully when two settings attributes share the same factory name

## [7.0.1] - 2018-10-18
### Changed
 - Incremented config version from 2.0 to 3.0 to reflect volume points changes

## [7.0.0] - 2018-09-14
### Changed
 - Originator constrained to class types
 - Performance improvements

## [6.2.1] - 2018-07-19
### Changed
 - Avoid loading SimplSharp assemblies from build directory in Net Standard
 - Removed try catch for accurate stack trace

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
