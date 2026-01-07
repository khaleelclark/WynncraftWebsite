import { createTheme, Theme } from "@mui/material/styles";
import type {} from "@mui/x-data-grid/themeAugmentation";

const imperial = {
  crimson: "#490113ff",
  blood: "#5b011bff",
  magenta: "#6a0b33ff",
  hotPink: "#E73C83",
  rose: "#F9A5C9",
  plum: "#310127ff",
  orchid: "#440638ff",
  paper: "#F7F2F5",
  pearl: "#E1CED3",
  ink: "#1A0B0E",
  dryTears: "#160101ff",
  ember: "#B0540F",
};

const imperialTable = {
  rowEven: "#240108ff",
  rowOdd: "#3c0010ff",
};

const theme = createTheme({
  palette: {
    primary: {
      main: imperial.magenta,
      dark: imperial.blood,
      light: imperial.hotPink,
      contrastText: imperial.paper,
    },
    secondary: {
      main: imperial.orchid,
      dark: imperial.plum,
      light: imperial.rose,
      contrastText: imperial.paper,
    },
    background: {
      default: imperial.dryTears,
      paper: imperial.crimson,
    },
    text: {
      primary: imperial.paper,
      secondary: imperial.rose,
    },
    divider: "rgba(225, 206, 211, 0.25)",
    error: { main: "#FF0055" },
    success: { main: "#4ADE80" }, // you can swap later if you want "imperial" success
    warning: { main: imperial.ember },

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
          backgroundColor: imperial.dryTears,
          color: imperial.paper,
        },
        a: {
          color: imperial.hotPink,
        },
      },
    },

    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: "none",
          backgroundColor: imperial.crimson,
          border: "1px solid rgba(178, 0, 75, 0.45)",
          boxShadow:
            "0 10px 24px rgba(0,0,0,0.45), inset 0 1px 0 rgba(249,165,201,0.08)",
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
          backgroundColor: imperial.ink,
          color: imperial.paper,
          border: "1px solid rgba(255, 214, 231, 0.55)",
          boxShadow: "0 0 14px rgba(231,60,131,0.20)",
          "&:hover": {
            backgroundColor: imperial.hotPink,
            boxShadow: "0 0 18px rgba(231,60,131,0.55)",
          },
        },

        outlinedPrimary: {
          color: imperial.paper,
          borderColor: "rgba(231, 60, 131, 0.55)",
          "&:hover": {
            borderColor: "rgba(231, 60, 131, 0.85)",
            backgroundColor: "rgba(231, 60, 131, 0.12)",
          },
        },

        textPrimary: {
          color: imperial.rose,
          "&:hover": {
            backgroundColor: "rgba(231, 60, 131, 0.10)",
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
          backgroundColor: "rgba(26, 11, 14, 0.35)", // ink wash
          borderRadius: 12,
          "& .MuiOutlinedInput-notchedOutline": {
            borderColor: "rgba(225, 206, 211, 0.25)",
          },
          "&:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: "rgba(231, 60, 131, 0.55)",
          },
          "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
            borderColor: "rgba(231, 60, 131, 0.85)",
            boxShadow: "0 0 0 3px rgba(231, 60, 131, 0.18)",
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
          color: "rgba(249, 165, 201, 0.85)",
          "&.Mui-focused": {
            color: imperial.hotPink,
          },
        },
      },
    },

    MuiFormHelperText: {
      styleOverrides: {
        root: {
          color: "rgba(225, 206, 211, 0.75)",
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
          backgroundColor: imperial.crimson,
          color: imperial.paper,
          border: "1px solid rgba(178, 0, 75, 0.65)",
          boxShadow:
            "0 10px 24px rgba(0,0,0,0.45), inset 0 1px 0 rgba(249,165,201,0.08)",
        },

        columnHeaders: {
          background: `linear-gradient(90deg, ${imperial.blood} 0%, ${imperial.magenta} 100%)`,
          color: imperial.paper,
          borderBottom: "1px solid rgba(231, 60, 131, 0.45)",
          fontWeight: 800,
        },

        cell: {
          borderColor: "rgba(225, 206, 211, 0.12)",
        },

        row: {
          "&:nth-of-type(even)": {
            backgroundColor: imperialTable.rowEven,
          },
          "&:nth-of-type(odd)": {
            backgroundColor: imperialTable.rowOdd,
          },

          // hover should tint, not replace
          // "&:hover": {
          //   backgroundColor: "rgba(231, 60, 131, 0.14)", // hot-pink wash
          // },
          "&:hover": {
            backgroundColor: "rgba(176, 84, 15, 0.16)", // ember wash
          },

          // selected should still feel selected but keep the vibe
          "&.Mui-selected": {
            backgroundColor: "rgba(178, 0, 75, 0.30)",
          },
          "&.Mui-selected:hover": {
            backgroundColor: "rgba(231, 60, 131, 0.22)",
          },
        },

        footerContainer: {
          backgroundColor: imperial.blood,
          color: imperial.paper,
          borderTop: "1px solid rgba(231, 60, 131, 0.45)",
        },
      },
    },

    MuiDivider: {
      styleOverrides: {
        root: {
          borderColor: "rgba(225, 206, 211, 0.18)",
        },
      },
    },

    MuiTooltip: {
      styleOverrides: {
        tooltip: {
          backgroundColor: imperial.ink,
          border: "1px solid rgba(231, 60, 131, 0.45)",
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
          background: `linear-gradient(90deg, ${imperial.plum} 0%, ${imperial.crimson} 40%, ${imperial.blood} 100%)`,
          borderBottom: "1px solid rgba(231, 60, 131, 0.35)",
        },
      },
    },

    MuiCard: {
      styleOverrides: {
        root: {
          backgroundImage: "none",
          backgroundColor: imperial.crimson,
          border: "1px solid rgba(178, 0, 75, 0.45)",
        },
      },
    },
  },
});

export default theme;
