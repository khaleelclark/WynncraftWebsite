import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import { ReactNode } from "react";
import Box from "@mui/material/Box";
import axios from "axios";
import { useState } from "react";
import { CustomFormContext } from "../CustomFormContext";
import Button from "@mui/material/Button";

interface CustomFormProps {
  title: string;
  children: ReactNode;
  apiEndpoint: string;
}

export const CustomForm = ({
  title,
  children,
  apiEndpoint,
}: CustomFormProps) => {
  const [formValues, setFormValues] = useState<Record<string, string>>({});

  const register = (id: string, value: string) => {
    setFormValues((prevValues) => ({ ...prevValues, [id]: value }));
  };

  const handleSubmit = async () => {
    await axios.post(apiEndpoint, formValues);
  };

  const el = (
    <CustomFormContext.Provider value={{ register, formValues }}>
      <Paper
        sx={{ backgroundColor: "#b94352ff", padding: 4, margin: 2 }}
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
          <Button
            variant="contained"
            sx={{ backgroundColor: "#220C0E" }}
            onClick={handleSubmit}
          >
            Submit
          </Button>
        </Box>
      </Paper>
    </CustomFormContext.Provider>
  );
  return el;
};
