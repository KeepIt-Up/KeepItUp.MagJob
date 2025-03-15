# Visual Design Guidelines

This document outlines the visual design system for the MagJob application. These guidelines ensure consistency across the entire application and should be followed for all new features and components.

## Table of Contents

- [Layout Principles](#layout-principles)
- [Color System](#color-system)
- [Typography](#typography)
- [Spacing System](#spacing-system)
- [Elevation & Shadows](#elevation--shadows)
- [Iconography](#iconography)
- [Responsive Design](#responsive-design)
- [Accessibility](#accessibility)

## Layout Principles

The MagJob application follows a structured layout with these key elements:

### Application Structure

- **Fixed Sidebar**: Navigation is positioned on the left side of the screen
- **Header Area**: Contains application title, search, and user profile
- **Content Area**: Main workspace that displays the active content
- **Card-Based Layout**: Content is organized into distinct card sections

### Visual Hierarchy

- Use consistent visual weight to establish importance
- Primary actions should be most prominent
- Group related information together
- Maintain adequate whitespace to create breathing room
- Use alignment to create order and clarity

### Content Organization

- Group related content into distinct sections
- Use clear headings to label each section
- Maintain consistent padding between sections (24px)
- Limit content density to improve readability

## Color System

### Primary Colors

| Color Name | Hex Code  | Usage                                |
| ---------- | --------- | ------------------------------------ |
| Primary    | `#44bba4` | Primary actions, buttons, highlights |
| Secondary  | `#ffdd00` | Accents, secondary elements          |
| Brand Red  | `#ff0000` | LEGO branding elements               |

### Neutral Colors

| Color Name  | Hex Code  | Usage                                |
| ----------- | --------- | ------------------------------------ |
| White       | `#ffffff` | Card backgrounds, primary surfaces   |
| Light Gray  | `#f8fafc` | Page backgrounds, secondary surfaces |
| Medium Gray | `#e2e8f0` | Borders, dividers                    |
| Dark Gray   | `#64748b` | Secondary text                       |
| Black       | `#1e293b` | Primary text                         |

### Semantic Colors

| Color Name | Hex Code  | Usage                             |
| ---------- | --------- | --------------------------------- |
| Success    | `#10b981` | Positive actions, confirmations   |
| Warning    | `#f59e0b` | Cautionary messages, alerts       |
| Error      | `#ef4444` | Error states, destructive actions |
| Info       | `#3b82f6` | Informational messages            |

### Color Usage Guidelines

- Use primary color for main actions and key interactive elements
- Maintain sufficient contrast ratios for accessibility (minimum 4.5:1 for text)
- Use semantic colors consistently for their intended purposes
- Limit color usage to the defined palette
- Apply colors consistently across similar elements

## Typography

### Font Family

- **Primary Font**: Inter or system sans-serif stack
- **Fallback**: `-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif`

### Type Scale

| Element | Size            | Weight         | Line Height | Usage             |
| ------- | --------------- | -------------- | ----------- | ----------------- |
| H1      | 24px (1.5rem)   | 700 (Bold)     | 1.2         | Page titles       |
| H2      | 20px (1.25rem)  | 600 (Semibold) | 1.2         | Section titles    |
| H3      | 18px (1.125rem) | 600 (Semibold) | 1.3         | Card titles       |
| Body    | 16px (1rem)     | 400 (Regular)  | 1.5         | Main content      |
| Small   | 14px (0.875rem) | 400 (Regular)  | 1.4         | Secondary content |
| Micro   | 12px (0.75rem)  | 400 (Regular)  | 1.4         | Labels, captions  |

### Typography Guidelines

- Maintain consistent text alignment (left-aligned for most content)
- Use appropriate text truncation for overflowing content
- Limit line length to 70-80 characters for optimal readability
- Use proper heading hierarchy (H1 → H2 → H3)
- Apply consistent text styles for similar content types

## Spacing System

### Spacing Scale

| Size | Value         | Usage                                    |
| ---- | ------------- | ---------------------------------------- |
| XS   | 4px (0.25rem) | Minimal spacing, tight elements          |
| SM   | 8px (0.5rem)  | Default spacing between related elements |
| MD   | 16px (1rem)   | Standard spacing between components      |
| LG   | 24px (1.5rem) | Spacing between sections                 |
| XL   | 32px (2rem)   | Large spacing for major sections         |
| XXL  | 48px (3rem)   | Extra large spacing                      |

### Layout Measurements

- **Sidebar width**: 240px
- **Header height**: 64px
- **Content max-width**: 1200px
- **Card padding**: 24px
- **Form field height**: 36px
- **Button height**: 36px (default)

### Spacing Guidelines

- Use consistent spacing between similar elements
- Apply spacing in multiples of the base unit (4px)
- Maintain adequate whitespace around content
- Use larger spacing to separate distinct sections
- Apply consistent padding within containers

## Elevation & Shadows

### Shadow Levels

| Level   | Usage                   | CSS                                |
| ------- | ----------------------- | ---------------------------------- |
| Level 1 | Cards, subtle elevation | `0 1px 3px rgba(0,0,0,0.1)`        |
| Level 2 | Dropdowns, popovers     | `0 4px 6px -1px rgba(0,0,0,0.1)`   |
| Level 3 | Modals, dialogs         | `0 10px 15px -3px rgba(0,0,0,0.1)` |
| Level 4 | Highest elevation       | `0 20px 25px -5px rgba(0,0,0,0.1)` |

### Elevation Guidelines

- Use shadows consistently to indicate elevation
- Higher elevation indicates more importance
- Apply appropriate shadow level based on element's purpose
- Maintain consistent shadow direction (top-left light source)
- Use subtle shadows for most elements to avoid visual noise

## Iconography

### Icon Style

- Use consistent line weight (1.5px stroke)
- Maintain uniform corner radius (2px)
- Use outlined icons for navigation and UI elements
- Use filled icons for active states and emphasis

### Icon Sizes

| Size   | Dimensions  | Usage                        |
| ------ | ----------- | ---------------------------- |
| Small  | 16px × 16px | Inline text, tight spaces    |
| Medium | 20px × 20px | Default for most UI elements |
| Large  | 24px × 24px | Navigation, emphasis         |

### Icon Guidelines

- Use icons consistently for similar actions
- Pair icons with text labels for clarity
- Maintain consistent alignment with text
- Use appropriate icon size based on context
- Ensure icons are recognizable and intuitive

## Responsive Design

### Breakpoints

| Breakpoint  | Range          | Target Devices          |
| ----------- | -------------- | ----------------------- |
| Small (SM)  | < 640px        | Mobile phones           |
| Medium (MD) | 641px - 1024px | Tablets, small laptops  |
| Large (LG)  | > 1024px       | Desktops, large laptops |

### Responsive Guidelines

- Design for mobile-first, then enhance for larger screens
- Use fluid layouts that adapt to different screen sizes
- Adjust spacing proportionally across breakpoints
- Simplify UI on smaller screens (hide secondary elements)
- Ensure touch targets are at least 44px × 44px on mobile
- Test designs across multiple device sizes

## Accessibility

### Color Contrast

- Maintain minimum contrast ratios:
  - 4.5:1 for normal text
  - 3:1 for large text (18px+ or 14px+ bold)
  - 3:1 for UI components and graphical objects

### Focus States

- All interactive elements must have visible focus states
- Focus indicators should be clearly visible (2px solid outline)
- Focus order should follow logical reading sequence

### Text Legibility

- Minimum text size of 16px for body content
- Avoid using text over busy backgrounds
- Ensure text remains legible when zoomed up to 200%

### Additional Guidelines

- Provide text alternatives for non-text content
- Design for keyboard navigation
- Ensure sufficient color contrast for all elements
- Support screen readers with proper ARIA attributes
- Test with accessibility tools regularly
