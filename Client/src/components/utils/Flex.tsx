import React from 'react';
import clsx from 'clsx';

type FlexProps = {
  direction?: 'row' | 'column';
  gap?: string;
  justifyContent?: 'center' | 'start' | 'end' | 'between' | 'around' | 'evenly';
  alignItems?: 'center' | 'start' | 'end' | 'stretch';
  className?: string;
  children: React.ReactNode;
};

const Flex: React.FC<FlexProps> = ({
  direction = 'row',
  gap = '4',
  justifyContent = 'start',
  alignItems = 'stretch',
  className,
  children,
}) => {
  return (
    <div
      className={clsx(
        'flex',
        direction === 'row' ? 'flex-row' : 'flex-col',
        `gap-${gap}`,
        `justify-${justifyContent}`,
        `items-${alignItems}`,
        className
      )}
    >
      {children}
    </div>
  );
};

export default Flex;
