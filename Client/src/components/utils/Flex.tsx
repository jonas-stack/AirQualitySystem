import React from 'react';

type FlexProps = {
  direction?: 'row' | 'column';
  gap?: string;
  justifyContent?: 'center' | 'start' | 'end' | 'between' | 'around' | 'evenly';
  alignItems?: 'center' | 'start' | 'end' | 'stretch';
  children: React.ReactNode;
};

const Flex: React.FC<FlexProps> = ({
  direction = 'row',
  gap = '4',
  justifyContent = 'start',
  alignItems = 'stretch',
  children,
}) => {
  return (
    <div
      className={`flex ${direction === 'row' ? 'flex-row' : 'flex-col'} 
                  gap-${gap} 
                  justify-${justifyContent} 
                  items-${alignItems}`}
    >
      {children}
    </div>
  );
};

export default Flex;
