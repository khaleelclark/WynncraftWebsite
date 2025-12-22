import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs, { Dayjs } from "dayjs";
import { useState } from "react";

//TODO: Add mui form control props

interface DateSelectorProps {
  id: string;
  name: string;
  required?: boolean;
}

export const CustomDateSelector = ({
  id,
  name,
  required,
}: DateSelectorProps) => {
  let el = <></>;
  return el;
};
