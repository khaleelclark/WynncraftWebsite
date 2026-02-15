import { createTheme, Theme } from "@mui/material/styles";
import type {} from "@mui/x-data-grid/themeAugmentation";

// Brand palette used across the site.
const imperial = {
  deepTeal: "#031A1A",
  darkTeal: "#052828",
  teal: "#083535",
  tealLight: "#0E4747",
  orange: "#F28C28",
  orangeDark: "#C96A12",
  gold: "#F4C542",
  goldLight: "#FFE08A",
  paper: "#FFF4D6",
  pearl: "#FFDFA3",
  ink: "#072222",
};

// Alternating table row colors for DataGrid.
const imperialTable = {
  rowEven: "#072626",
  rowOdd: "#051F1F",
};

// Global MUI theme overrides and component styles.
const theme = createTheme({
  palette: {
    primary: {
      main: imperial.orange,
      dark: imperial.orangeDark,
      light: imperial.goldLight,
      contrastText: imperial.ink,
    },
    secondary: {
      main: imperial.gold,
      dark: imperial.orangeDark,
      light: imperial.goldLight,
      contrastText: imperial.ink,
    },
    background: {
      default: imperial.deepTeal,
      paper: imperial.darkTeal,
    },
    text: {
      primary: imperial.paper,
      secondary: imperial.pearl,
    },
    divider: "rgba(242, 140, 40, 0.45)",
    error: { main: "#FF0055" },
    success: { main: "#4ADE80" },
    warning: { main: imperial.gold },

    info: { main: "#60A5FA" },
  },

  shape: { borderRadius: 12 },

  typography: {
    fontFamily: `"Inter", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif`,
    h4: {
      fontWeight: 800,
    },
    h5: {
      fontWeight: 700,
    },
    h6: {
      fontWeight: 700,
    },
    button: { textTransform: "none", fontWeight: 700 },
  },

  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: {
          backgroundColor: imperial.deepTeal,
          color: imperial.paper,
        },
        a: {
          color: imperial.gold,
        },
      },
    },

    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: "none",
          backgroundColor: imperial.darkTeal,
          border: "1px solid rgba(242, 140, 40, 0.65)",
          boxShadow:
            "0 10px 24px rgba(0,0,0,0.45), inset 0 1px 0 rgba(242,140,40,0.14)",
        },
      },
    },

    MuiButton: {
      defaultProps: {
        disableElevation: true,
      },
      styleOverrides: {
        root: {
          borderRadius: 10,
          paddingInline: 14,
          paddingBlock: 9,
        },

        containedPrimary: {
          backgroundColor: imperial.orange,
          color: imperial.ink,
          border: "1px solid rgba(242, 140, 40, 0.75)",
          boxShadow: "0 0 14px rgba(242,140,40,0.28)",
          "&:hover": {
            backgroundColor: imperial.orangeDark,
            color: imperial.paper,
            boxShadow: "0 0 20px rgba(242,140,40,0.55)",
          },
        },

        outlinedPrimary: {
          color: imperial.paper,
          borderColor: "rgba(242, 140, 40, 0.65)",
          "&:hover": {
            borderColor: "rgba(242, 140, 40, 0.95)",
            backgroundColor: "rgba(242, 140, 40, 0.2)",
          },
        },

        textPrimary: {
          color: imperial.pearl,
          "&:hover": {
            backgroundColor: "rgba(242, 140, 40, 0.2)",
          },
        },
      },
    },

    MuiTypography: {
      styleOverrides: {
        root: {
          color: "inherit",
        },
      },
    },

    MuiTextField: {
      defaultProps: {
        variant: "outlined",
        fullWidth: true,
      },
    },

    MuiOutlinedInput: {
      styleOverrides: {
        root: {
          backgroundColor: "rgba(7, 34, 34, 0.45)", // ink wash
          borderRadius: 12,
          "& .MuiOutlinedInput-notchedOutline": {
            borderColor: "rgba(255, 223, 163, 0.25)",
          },
          "&:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: "rgba(242, 140, 40, 0.65)",
          },
          "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
            borderColor: "rgba(242, 140, 40, 0.95)",
            boxShadow: "0 0 0 3px rgba(242, 140, 40, 0.24)",
          },
        },
        input: {
          color: imperial.paper,
        },
      },
    },

    MuiInputLabel: {
      styleOverrides: {
        root: {
          color: "rgba(255, 208, 140, 0.92)",
          "&.Mui-focused": {
            color: imperial.orange,
          },
        },
      },
    },

    MuiFormHelperText: {
      styleOverrides: {
        root: {
          color: "rgba(255, 196, 112, 0.82)",
        },
      },
    },

    MuiAutocomplete: {
      styleOverrides: {
        paper: ({ theme }: { theme: Theme }) => ({
          backgroundColor: theme.palette.background.paper,
          color: theme.palette.text.primary,
          border: `1px solid ${theme.palette.divider}`,
          borderRadius: 12,
          boxShadow: "0 20px 60px rgba(0,0,0,0.65)",
          overflow: "hidden",
        }),

        listbox: ({ theme }: { theme: Theme }) => ({
          padding: 6,
          "& .MuiAutocomplete-option": {
            borderRadius: 10,
            margin: "2px 0",
            "&[aria-selected='true']": {
              backgroundColor: `${theme.palette.primary.main}33`,
            },
            "&.Mui-focused": {
              backgroundColor: `${theme.palette.primary.main}22`,
            },
          },
        }),

        tag: ({ theme }: { theme: Theme }) => ({
          backgroundColor: `${theme.palette.primary.main}22`,
          border: `1px solid ${theme.palette.primary.main}55`,
          color: theme.palette.text.primary,
          borderRadius: 10,
        }),
      },
    },

    MuiChip: {
      styleOverrides: {
        deleteIcon: ({ theme }: { theme: Theme }) => ({
          color: theme.palette.text.secondary,
          "&:hover": { color: theme.palette.text.primary },
        }),
      },
    },

    MuiDataGrid: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          overflow: "hidden",
          backgroundColor: imperial.darkTeal,
          color: imperial.paper,
          border: "1px solid rgba(242, 140, 40, 0.68)",
          boxShadow:
            "0 10px 24px rgba(0,0,0,0.45), inset 0 1px 0 rgba(242,140,40,0.12)",
          "& .MuiDataGrid-menuIcon, & .MuiDataGrid-menuIconButton": {
            color: imperial.paper,
            opacity: 1,
          },
          "& .MuiDataGrid-menuIconButton:hover": {
            backgroundColor: "rgba(242, 140, 40, 0.28)",
          },
        },

        columnHeaders: {
          background: `linear-gradient(90deg, ${imperial.teal} 0%, ${imperial.tealLight} 100%)`,
          color: imperial.paper,
          borderBottom: "1px solid rgba(242, 140, 40, 0.55)",
          fontWeight: 800,
        },

        cell: {
          borderColor: "rgba(242, 140, 40, 0.2)",
        },

        row: {
          "&:nth-of-type(even)": {
            backgroundColor: imperialTable.rowEven,
          },
          "&:nth-of-type(odd)": {
            backgroundColor: imperialTable.rowOdd,
          },
          "&:hover": {
            backgroundColor: "rgba(242, 140, 40, 0.2)",
          },
          "&.Mui-selected": {
            backgroundColor: "rgba(242, 140, 40, 0.34)",
          },
          "&.Mui-selected:hover": {
            backgroundColor: "rgba(242, 140, 40, 0.42)",
          },
        },
      },
    },

    MuiDivider: {
      styleOverrides: {
        root: {
          borderColor: "rgba(242, 140, 40, 0.4)",
        },
      },
    },

    MuiTooltip: {
      styleOverrides: {
        tooltip: {
          backgroundColor: imperial.ink,
          border: "1px solid rgba(242, 140, 40, 0.65)",
          color: imperial.paper,
        },
        arrow: {
          color: imperial.ink,
        },
      },
    },

    MuiAppBar: {
      styleOverrides: {
        root: {
          background: `linear-gradient(90deg, ${imperial.teal} 0%, ${imperial.darkTeal} 45%, ${imperial.deepTeal} 100%)`,
          borderBottom: "1px solid rgba(242, 140, 40, 0.55)",
        },
      },
    },

    MuiCard: {
      styleOverrides: {
        root: {
          backgroundImage: "none",
          backgroundColor: imperial.darkTeal,
          border: "1px solid rgba(242, 140, 40, 0.65)",
        },
      },
    },
  },
});

export default theme;
