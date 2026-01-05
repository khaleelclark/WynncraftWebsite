import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import { ReactNode } from "react";
import Box from "@mui/material/Box";

import { useState } from "react";
import Button from "@mui/material/Button";
import React from "react";

export interface CustomFormProps {
  title: string;
  children: ReactNode[] | ReactNode;
  apiEndpoint: string;
  onSubmitSuccess?: () => void;
  onSubmitError?: (error: unknown) => void;
  changeRecordsCallback?: (newRecord: Object) => void;
  autofillData?: any;
  isPost?: boolean; //true = post, false = put
}

export interface FormRegisterProps {
  id: string;
  label: string;
  required?: boolean;
  register?: (id: string, value: any, validation: boolean) => void;
  formValues?: Record<string, any>;
  formValidations?: Record<string, any>;
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
  const [formValues, setFormValues] = useState<Record<string, any>>(
    autofillData ?? {}
  );
  const [formValidations, setFormValidations] = useState<Record<string, any>>(
    {}
  );

  const register = (id: string, value: any, validation: boolean) => {
    setFormValues(prevValues => ({ ...prevValues, [id]: value }));
    setFormValidations(prevValues => ({ ...prevValues, [id]: validation }));
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
        await adminApi
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
    <Paper
      sx={{
        padding: 4,
        margin: 2,
        bgcolor: "background.paper",
        color: "text.primary",
        borderRadius: 3,
        border: theme => `1px solid ${theme.palette.divider}`,
      }}
      elevation={3}
    >
      <Typography
        variant="h4"
        sx={{
          mb: 3,
          fontWeight: 800,
          color: "text.primary",
          textAlign: "center",
        }}
      >
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
        {React.Children.map(children, child => {
          if (!React.isValidElement(child)) return child;
          return React.cloneElement(
            child as React.ReactElement<FormRegisterProps>,
            {
              register,
              formValues,
              formValidations,
            }
          );
        })}
        <Button
          variant="contained"
          color="primary"
          onClick={handleSubmit}
          disabled={!Object.values(formValidations).every(Boolean)}
          sx={{
            px: 3,

            // optional: extra glow without hardcoding hex
            boxShadow: theme => `0 0 16px ${theme.palette.primary.main}33`,
            "&:hover": {
              boxShadow: theme => `0 0 22px ${theme.palette.primary.main}66`,
            },
          }}
        >
          Submit
        </Button>
      </Box>
    </Paper>
  );
  return el;
};
