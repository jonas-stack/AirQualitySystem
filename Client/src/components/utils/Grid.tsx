import { FC, ReactNode } from 'react';
import clsx from 'clsx';

type GridProps = {
  columns?: string;
  rows?: string;
  gap?: string;
  minHeight?: string;
  justifyItems?: 'start' | 'center' | 'end' | 'stretch';
  alignItems?: 'start' | 'center' | 'end' | 'stretch';
  className?: string;
  children: ReactNode;
};

const Grid: FC<GridProps> = ({
  columns = 'grid-cols-1',
  rows = '',
  gap = '4',
  minHeight = 'min-h-screen',
  justifyItems = 'stretch',
  alignItems = 'stretch',
  className,
  children,
}) => {
  return (
    <div
      className={clsx(
        'grid',
        columns,
        rows,
        `gap-${gap}`,
        minHeight,
        `justify-items-${justifyItems}`,
        `items-${alignItems}`,
        className
      )}
    >
      {children}
    </div>
  );
};

export default Grid;
