// space separotor follows ant design principles
// sm: between text or label and input
// m: between proximity component
// lg: to separate components with different meaning
// xl: more space
// {{value: "custom value"}}
import React from 'react';

interface ISpacings {
  sm: string;
  m: string;
  lg: string;
  xl: string;
}
const spacings: ISpacings = {
  sm: "8px",
  m: "16px",
  lg: "24px",
  xl: "32px"
};

type SpaceSeparatorProps = {
  size?: keyof ISpacings | { value: string };
  type?: "vertical" | "horizontal";
};

export default function SpaceSeparator(props: SpaceSeparatorProps) {
  const { size = "m", type = "horizontal" } = props;
  let finalStyles: React.CSSProperties = {};
  if (type === "horizontal") {
    finalStyles = {
      paddingTop: getSize(size)
    };
  } else {
    finalStyles = {
      paddingLeft: getSize(size)
    };
  }
  return <div style={finalStyles}></div>;
}
const getSize = (size: keyof ISpacings | { value: string }): string => {
  if (size instanceof Object) {
    return size.value;
  }
  if (size in spacings) {
    return spacings[size];
  }
  return size;
};
