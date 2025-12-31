import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import { ReactNode, useEffect } from "react";
import Box from "@mui/material/Box";
import axios from "axios";
import { useState } from "react";
import { CustomFormContext } from "../CustomFormContext";
import Button from "@mui/material/Button";

export interface CustomFormProps {
  title: string;
  children: ReactNode;
  apiEndpoint: string;
  onSubmitSuccess?: () => void;
  onSubmitError?: (error: unknown) => void;
  changeRecordsCallback?: (newRecord: Object) => void;
  autofillData?: any;
  isPost?: boolean; //true = post, false = put
}

export const CustomForm = ({
  title,
  children,
  apiEndpoint,
  onSubmitSuccess,
  onSubmitError,
  changeRecordsCallback,
  autofillData,
  isPost,
}: CustomFormProps) => {
  const [formValues, setFormValues] = useState<Record<string, string>>(
    autofillData ?? {}
  );

  useEffect(() => {}, []);

  const register = (id: string, value: string) => {
    setFormValues(prevValues => ({ ...prevValues, [id]: value }));
  };

  const flattenObject = (obj: any) => {
    return Object.fromEntries(
      Object.entries(obj).map(([key, value]) => {
        // 🔹 If it's an array, flatten each element the same way you flatten objects
        if (Array.isArray(value)) {
          return [
            key,
            value.map(
              v =>
                v && typeof v === "object"
                  ? (v as any).id ?? v // if v has an id, use it
                  : v // otherwise leave as-is (primitives)
            ),
          ];
        }

        // 🔹 If it's an object (not array), use its id if available
        if (value && typeof value === "object") {
          return [key, (value as any).id ?? value];
        }

        // 🔹 Otherwise just return the primitive value
        return [key, value];
      })
    );
  };

  const handleSubmit = async () => {
    const flattenFormValues = flattenObject(formValues);
    try {
      if (isPost) {
        await axios.post(apiEndpoint, flattenFormValues).then(res => {
          if (changeRecordsCallback) changeRecordsCallback(res.data);
        });
      } else {
        await axios
          .put(`${apiEndpoint}/${formValues.id}`, flattenFormValues)
          .then(res => {
            if (changeRecordsCallback) changeRecordsCallback(res.data);
          });
      }

      if (onSubmitSuccess) {
        onSubmitSuccess();
      }
    } catch (error) {
      console.error("Form submission failed:", error);
      if (onSubmitError) {
        onSubmitError(error);
      }
    }
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
