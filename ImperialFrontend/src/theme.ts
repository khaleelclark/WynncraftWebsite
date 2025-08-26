import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    primary: {
      main: '#82172e',
      dark: '#511220',
      light: '#bc511c',
      contrastText: '#efdddb',
    },
    secondary: {
      main: '#a63024',
      dark: '#220c0e',
      contrastText: '#efdddb',
    },
    background: {
      default: '#220c0e',
      paper: '#511220',
    },
    text: {
      primary: '#efdddb',
      secondary: '#bc511c',
    },
  },
  components: {
    MuiDataGrid: {
      styleOverrides: {
        root: {
          backgroundColor: '#511220',
          color: '#efdddb',
          border: '1px solid #82172e',
        },
        columnHeaders: {
          backgroundColor: '#82172e',
          color: '#efdddb',
        },
        row: {
          '&:nth-of-type(even)': {
            backgroundColor: '#220c0e',
          },
          '&:nth-of-type(odd)': {
            backgroundColor: '#511220',
          },
        },
      },
    },
  },
});

export default theme;
