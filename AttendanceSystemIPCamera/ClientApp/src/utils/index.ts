import format from 'date-fns/format';
import moment from 'moment';

export const formatFullDateTimeString = (time: Date | string) => format(new Date(time), 'EEEE, MMMM d, yyyy');
export const minutesOfDay = (time: moment.Moment) => {
    return time.minutes() + time.hours() * 60;
}