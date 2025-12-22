import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import { ReactNode } from "react";
import Box from "@mui/material/Box";

interface CustomFormProps {
  title: string;
  children: ReactNode;
}

export const CustomForm = ({ title, children }: CustomFormProps) => {
  const el = (
    <div>
      <Paper
        sx={{ backgroundColor: "#802834ff", padding: 4, margin: 2 }}
        elevation={3}
      >
        <Typography color="#000000ff" variant="h3" mb={3} fontWeight={600}>
          {title}
        </Typography>

        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            gap: 3,
            alignItems: "stretch",
            padding: 2,
            margin: 2,
          }}
        >
          {children}
        </Box>
      </Paper>
    </div>
  );
  return el;
};
