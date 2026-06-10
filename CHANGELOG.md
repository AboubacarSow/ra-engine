# Changelog

All notable changes to **KCodd** will be documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [Unreleased]

### Planned
- Outer joins
- LaTeX rendering
- AST visualization and its optimization
- Explicite error message
- Tests for each layer(Lexer, Parser, SqlGenerator)

---
## [1.0.1] - 2026-06-10

### Added

* Cartesian product (×) operator
* Division (÷) operator
* Transformer layer for query processing and transformation

### Fixed

* Nested expression parsing and evaluation issues causing failures at deeper nesting levels (e.g., `Expected RPAREN, got EOF`)
* Improved robustness of deeply nested relational algebra expressions

### Improved

* Overall stability and reliability of the query pipeline



## [1.0.0] - 2026-05-16

### Added
- Relational Algebra → SQL transpilation engine
- Lexer, parser, and AST pipeline
- SQL generation from AST
- Interactive Blazor playground
- CLI playground
- Selection, projection,rename, natural join,theta join, union, intersection, and difference operators
- Query history support
- Dark/light theme support
- Unit and end-to-end tests

