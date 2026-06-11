# GitHub Copilot Instructions for RimWorld Mod: Radiology (Continued)

## Mod Overview and Purpose

Radiology (Continued) is a modification for the game RimWorld that introduces a system for exposing colonists and prisoners to harmful radiation, potentially resulting in either beneficial mutations or harmful effects such as burns and cancer. The mod enhances gameplay by allowing players to invest in research and construction to harness the potential of radiation for managing or enhancing their colonists.

## Key Features and Systems

- **Research Integration**: Access new functionalities through a dedicated research tree tab.
- **Architect Tab Additions**: Build specialized structures such as the Chamber and Irradiator for conducting radiation experiments.
- **Radiation Management**: Use filters to modify radiation types and blockers to select target body parts.
- **Mutation Mechanics**: Experience a range of mutations affecting colonists, including skill alterations and regeneration abilities.
- **Comprehensive Support**: Features are designed to seamlessly integrate into existing saves and support both addition and removal.

## Coding Patterns and Conventions

- **Naming Conventions**: Use PascalCase for classes and methods, camelCase for local variables, and constants in ALL_CAPS.
- **Documentation**: Ensure every class and significant method is well-documented to describe functionality and parameters.
- **Code Structure**: Keep related functionalities grouped in namespaces, reflecting game systems like mutations and radiation handling.

## XML Integration

- **Definition Files**: Use XML to define game objects, recipes, and categories, ensuring clear links between objects and their in-game representations.
  - Examples include `ThingDef` in Buildings.xml and `HediffDef` in MutationsBlink.xml.
- **Modular Design**: Organize XML files for specific purposes like buildings, hediffs, and designations to facilitate updates and maintenance.

## Harmony Patching

- **Harmony Library**: Utilize the Harmony library for method patching to modify existing game behaviors without altering core game files.
- **Patch Targeting**: Carefully select methods for Prefix, Postfix, and Transpiler patches to ensure they align with mod functionality.

## Suggestions for Copilot

- **Autocomplete Definitions**: Assist in autocompleting XML tags and attributes, consistent with RimWorld's modding framework.
- **Harmony Insights**: Suggest appropriate methods and locations for patches within game assemblies, minimizing conflict risks.
- **Template Generation**: Provide templates for common modding patterns, such as new building or hediff definitions, reducing initial setup time.
- **Consistent Refactoring**: Identify opportunities for code standardization across the project, ensuring a cohesive coding style.

By following these instructions and utilizing GitHub Copilot effectively, mod developers can enhance their productivity and maintain high-quality code within the Radiology mod project for RimWorld.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

