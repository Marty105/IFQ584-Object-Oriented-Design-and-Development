<!--
================================================================================
IFQ584 Object-Oriented Design and Development — Assignment 2 Report
Numerical Tic Tac Toe (C# / .NET console application)

STATUS: TEMPLATE — front-matter + section scaffold. Fill the _TODO_ blocks.

SUBMISSION FORMAT (from brief — enforce before export):
  - ONE PDF, max 12 pages, A4, 2cm margins all sides.
  - 12pt Times New Roman OR 11pt Arial (or equivalent), single spaced.
  - Executive summary: up to 2 pages.
  - Must include: team member list (names, student numbers, emails) + team number;
    declaration of contributions per member; statement of completion.
  NB: any member who didn't contribute substantially receives ZERO — contributions
  declaration must be honest and specific.

PATTERNS TO DEMONSTRATE (agreed): Factory, Singleton, Command.
  See §6 for where each maps into the existing design (Program / Game / Player /
  Computer / Board / Move from Assignment 1 Design.md).

EXPORT (matches memory: reportlab renders clean; avoid Chrome-headless PDFs):
  pandoc Report.md -o Report.pdf \
    -V geometry:a4paper -V geometry:margin=2cm \
    -V mainfont="Times New Roman" -V fontsize=12pt
  Then confirm page count <= 12 and visually check the mermaid/code renders.
================================================================================
-->

# IFQ584 Object-Oriented Design and Development
## Assignment 2 — Numerical Tic Tac Toe

**Team number:** _TODO_

---

## Team Members

| Full name | Student number | Email |
|-----------|---------------|-------|
| Lucas Cullen | n7641052 | _TODO@connect.qut.edu.au_ |
| _TODO_ | _TODO_ | _TODO_ |
| _TODO_ | _TODO_ | _TODO_ |
| _TODO_ | _TODO_ | _TODO_ |

---

## Declaration of Contributions

<!-- Be specific and honest. A member who did not contribute substantially
receives ZERO. Attribute concrete deliverables (which classes, patterns,
diagrams, tests, sections) to each person, and give an overall % or equal split
with justification. -->

| Team member | Contribution (specific deliverables) | Approx. share |
|-------------|--------------------------------------|---------------|
| Lucas Cullen | _TODO — e.g. Command pattern (undo/redo), Game refactor, this report §6_ | _TODO %_ |
| _TODO_ | _TODO_ | _TODO %_ |
| _TODO_ | _TODO_ | _TODO %_ |
| _TODO_ | _TODO_ | _TODO %_ |

_All team members contributed substantially. Signed / acknowledged: TODO._

---

## Statement of Completion

<!-- MUST be complete and accurate — this is directly marked under 'Fulfilment
of requirements'. List every requirement from the brief and mark its status.
Do NOT claim something is done if it is partial — say what remains. -->

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| 1 | Numerical Tic Tac Toe core gameplay (n×n, shared 1..n², magic-constant win) | _Implemented / Partial / Not implemented_ | _TODO_ |
| 2 | Human vs human | _..._ | _TODO_ |
| 3 | Human vs computer | _..._ | _TODO_ |
| 4 | Save / load game (JSON) | _..._ | _TODO_ |
| 5 | Undo / redo of moves | _..._ | _TODO_ |
| 6 | **Factory** pattern applied | _..._ | _where — see §6.1_ |
| 7 | **Singleton** pattern applied | _..._ | _where — see §6.2_ |
| 8 | **Command** pattern applied | _..._ | _where — see §6.3_ |
| 9 | _Other brief requirement — TODO confirm against unit spec_ | _..._ | _TODO_ |

_Declaration: the above is a complete and accurate statement of what has and has
not been implemented._

---

## Executive Summary
<!-- UP TO 2 PAGES. Write LAST. Orient the reader: what the application is, the
OO design approach, the three design patterns applied and the benefit each
brought, the completion status at a glance, and how the team worked. Non-code,
decision-focused — the marker should understand the whole submission from this
alone. -->

_TODO_

---

## 1. Introduction and Scope

<!-- Brief. What Numerical Tic Tac Toe is, what this deliverable covers
(implementation + patterns beyond the Assignment 1 design), and how the report is
structured. Reference Assignment 1 Design.md rather than repeating the full
domain description. -->

_TODO_

---

## 2. Design Overview

<!-- Summarise the class model carried from Assignment 1 (Game, Board, Piece,
Move, IPlayer/PlayerBase/Player/Computer, IGame, Program) and any changes made in
Assignment 2. A trimmed class diagram helps; don't reproduce all of Design.md —
reference it and highlight what changed to accommodate the three patterns. -->

_TODO_

---

## 3. Implementation Notes

<!-- Language/runtime (C# / .NET), project layout, how to build and run, and any
notable implementation decisions. Keep tight — pages are scarce. -->

_TODO — e.g._
```
dotnet run --project TicTacToe
```

---

## 4. Testing

<!-- How correctness was checked: unit tests, manual test scenarios (win
detection, undo/redo round-trip, save/load round-trip, computer blocking/winning
move). Table of test cases + result is efficient here. -->

_TODO_

---

## 5. Object-Oriented Principles

<!-- Short — where encapsulation, inheritance, polymorphism, abstraction show up.
PlayerBase→Player/Computer polymorphism, IGame/IPlayer abstraction, Piece
encapsulating value→mark. Ties the marking criterion for OO quality. -->

_TODO_

---

## 6. Design Patterns Applied

The design applies three Gang-of-Four patterns. Each subsection states the
**intent**, **where** it is applied in this codebase, **why** it fits, and the
**benefit** over the naive alternative.

### 6.1 Factory

<!-- Intent: encapsulate object creation so callers don't hard-code concrete
types. Natural home in this codebase: PLAYER CREATION. Program currently
constructs Player vs Computer inline from the menu choice; a PlayerFactory
(or factory method) centralises "given a player kind + name, return an IPlayer".
Extends cleanly if more player types are added (e.g. networked, smarter AI).
Could also apply to Board/Game construction for a given board size n. -->

- **Intent:** _TODO_
- **Applied to:** _player creation (IPlayer) — replaces inline `new Player()` /
  `new Computer()` in `Program`._
- **Why it fits / benefit:** _TODO — decouples the game loop from concrete player
  types; single place to add a new player kind._

```csharp
// TODO: PlayerFactory.Create(PlayerKind kind, string name) => IPlayer
```

### 6.2 Singleton

<!-- Intent: exactly one instance with a global access point. Candidate homes:
a single game-configuration/settings holder, a console-I/O wrapper, or a
game-logger. Pick ONE that is genuinely single-instance — do NOT force it onto
Game (Game is not naturally a singleton and markers dislike misapplied
singletons). A ConsoleIO / GameSettings singleton is the defensible choice.
Note thread-safety briefly (single-threaded console app => simple). -->

- **Intent:** _TODO_
- **Applied to:** _TODO — e.g. `GameSettings` or `ConsoleIO` single instance._
- **Why it fits / benefit:** _TODO — one shared configuration/IO surface;
  avoids passing it through every constructor._
- **Caveat considered:** _why this is a legitimate singleton and not a global
  variable in disguise._

```csharp
// TODO: sealed class with private ctor + static Instance
```

### 6.3 Command

<!-- Intent: encapsulate a request as an object, enabling parameterisation,
queuing, logging, and — crucially here — UNDO/REDO. This is the strongest fit:
the existing undo/redo already implies command history. Model each move as a
MoveCommand with Execute()/Undo(); Game holds an undo stack and a redo stack of
ICommand instead of raw Move values. Ties directly to Design.md §6 undo/redo
notes and sequence/object diagrams. -->

- **Intent:** _TODO_
- **Applied to:** _move execution + undo/redo — `ICommand` / `MoveCommand` with
  `Execute()` and `Undo()`, replacing the raw `Stack<Move>` redo model._
- **Why it fits / benefit:** _TODO — natural undo/redo, extensible to other
  actions, cleaner separation of "what a move does" from the game loop._

```csharp
// TODO: interface ICommand { void Execute(); void Undo(); }
//       class MoveCommand : ICommand { ... }
```

---

## 7. Reflection / Limitations

<!-- Honest: what would you improve, known limitations, what was cut for scope.
Aligns with the Statement of Completion above. -->

_TODO_

---

## Appendix — Team Working Notes
<!-- Not for submission page count if brief allows; otherwise trim. Meeting log,
task board, GenAI acknowledgement per QUT Cite|Write if any AI assistance used. -->

- **GenAI acknowledgement:** _TODO if applicable._
- **Task allocation log:** _TODO._
