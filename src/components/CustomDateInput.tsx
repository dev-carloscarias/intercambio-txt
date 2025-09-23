import React, { useState, useRef, useEffect } from 'react';
import './CustomDateInput.scss';

interface CustomDateInputProps {
    label: string;
    placeholder: string;
    value: Date | null;
    onChangeDate: (date: Date | null) => void;
    onClear: () => void;
}

const CustomDateInput: React.FC<CustomDateInputProps> = ({
    label,
    placeholder,
    value,
    onChangeDate,
    onClear
}) => {
    const [isOpen, setIsOpen] = useState(false);
    const [inputValue, setInputValue] = useState('');
    const [displayValue, setDisplayValue] = useState('');
    const inputRef = useRef<HTMLInputElement>(null);
    const calendarRef = useRef<HTMLDivElement>(null);

    // Formatear fecha para mostrar
    const formatDateForDisplay = (date: Date | null): string => {
        if (!date) return '';
        return date.toLocaleDateString('en-US', {
            month: '2-digit',
            day: '2-digit',
            year: 'numeric'
        });
    };

    // Formatear fecha para input
    const formatDateForInput = (date: Date | null): string => {
        if (!date) return '';
        return date.toISOString().split('T')[0];
    };

    // Actualizar valores cuando cambie el prop value
    useEffect(() => {
        if (value) {
            setDisplayValue(formatDateForDisplay(value));
            setInputValue(formatDateForInput(value));
        } else {
            setDisplayValue('');
            setInputValue('');
        }
    }, [value]);

    // Manejar cambio en el input de texto
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const text = e.target.value;
        setInputValue(text);
        setDisplayValue(text);

        // Intentar parsear la fecha
        if (text.trim()) {
            const date = new Date(text);
            if (!isNaN(date.getTime())) {
                setDisplayValue(formatDateForDisplay(date));
                onChangeDate(date);
            }
        } else {
            onChangeDate(null);
        }
    };

    // Manejar selección de fecha del calendario
    const handleDateSelect = (date: Date) => {
        setDisplayValue(formatDateForDisplay(date));
        setInputValue(formatDateForInput(date));
        onChangeDate(date);
        setIsOpen(false);
    };

    // Manejar clic en el botón de calendario
    const handleCalendarClick = () => {
        setIsOpen(!isOpen);
    };

    // Manejar clic en limpiar
    const handleClear = () => {
        setDisplayValue('');
        setInputValue('');
        onChangeDate(null);
        onClear();
    };

    // Cerrar calendario al hacer clic fuera
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (calendarRef.current && !calendarRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    // Generar días del mes
    const generateCalendarDays = (date: Date) => {
        const year = date.getFullYear();
        const month = date.getMonth();
        const firstDay = new Date(year, month, 1);
        const lastDay = new Date(year, month + 1, 0);
        const daysInMonth = lastDay.getDate();
        const startingDayOfWeek = firstDay.getDay();

        const days: (Date | null)[] = [];
        
        // Días del mes anterior
        for (let i = 0; i < startingDayOfWeek; i++) {
            days.push(null);
        }
        
        // Días del mes actual
        for (let day = 1; day <= daysInMonth; day++) {
            days.push(new Date(year, month, day));
        }
        
        return days;
    };

    const currentDate = value || new Date();
    const calendarDays = generateCalendarDays(currentDate);

    return (
        <div className="custom-date-input">
            <div className="form-group">
                <label className="form-label fw-bold text-primary">{label}</label>
            <div className="input-container">
                <input
                    ref={inputRef}
                    type="text"
                    className="form-control date-input"
                    placeholder={placeholder}
                    value={displayValue}
                    onChange={handleInputChange}
                    onFocus={() => setIsOpen(true)}
                />
                <div className="input-icons">
                    <button
                        type="button"
                        className="calendar-icon"
                        onClick={handleCalendarClick}
                    >
                        <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                            <path d="M3.5 0a.5.5 0 0 1 .5.5V1h6V.5a.5.5 0 0 1 1 0V1h1a2 2 0 0 1 2 2v11a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V3a2 2 0 0 1 2-2h1V.5a.5.5 0 0 1 .5-.5zM1 4v10a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V4H1z"/>
                        </svg>
                    </button>
                </div>
            </div>
            </div>
            
            {isOpen && (
                <div ref={calendarRef} className="custom-calendar">
                    <div className="calendar-header">
                        <button
                            type="button"
                            className="btn btn-sm btn-outline-secondary"
                            onClick={() => {
                                const newDate = new Date(currentDate);
                                newDate.setMonth(newDate.getMonth() - 1);
                                setDisplayValue(formatDateForDisplay(newDate));
                            }}
                        >
                            ‹
                        </button>
                        <span className="calendar-month">
                            {currentDate.toLocaleDateString('en-US', { month: 'long', year: 'numeric' })}
                        </span>
                        <button
                            type="button"
                            className="btn btn-sm btn-outline-secondary"
                            onClick={() => {
                                const newDate = new Date(currentDate);
                                newDate.setMonth(newDate.getMonth() + 1);
                                setDisplayValue(formatDateForDisplay(newDate));
                            }}
                        >
                            ›
                        </button>
                    </div>
                    <div className="calendar-weekdays">
                        <div className="weekday">Su</div>
                        <div className="weekday">Mo</div>
                        <div className="weekday">Tu</div>
                        <div className="weekday">We</div>
                        <div className="weekday">Th</div>
                        <div className="weekday">Fr</div>
                        <div className="weekday">Sa</div>
                    </div>
                    <div className="calendar-days">
                        {calendarDays.map((day, index) => (
                            <button
                                key={index}
                                type="button"
                                className={`calendar-day ${day ? '' : 'empty'} ${
                                    day && value && day.toDateString() === value.toDateString() ? 'selected' : ''
                                }`}
                                onClick={() => day && handleDateSelect(day)}
                                disabled={!day}
                            >
                                {day ? day.getDate() : ''}
                            </button>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};

export default CustomDateInput;
