import React from 'react';
import { shallow } from 'enzyme';
import BottomSheetCancelNext from './BottomSheetCancelNext';

describe('BottomSheetCancelNext component', () => {
    it('should render the component', () => {
        const props = {
            isOpen: false,
            onClose: () => {},
            onCancel: () => {},
            onNext: () => {}
        };
        const wrapper = shallow(<BottomSheetCancelNext {...props} />);
        expect(wrapper).toBeTruthy();
    });
});
