import React from 'react';
import { shallow } from 'enzyme';
import RequestingProviderSelectedItem from './RequestingProviderSelectedItem';

describe('RequestingProviderSelectedItem component', () => {
    it('should render the component', () => {
        const props = {
            item: {
                npi: '123456',
                name: 'John Doe',
                phoneNumber: '1234',
                email: 'mail@mail.com',
                specialties: [],
                autoSelectCity: false,
                id: 22,
                renderingProviderId: 1234,
                renderingProviderNPI: 'string',
                renderingProviderName: 'string'
            },
            onSelectCity: () => {}
        };
        const wrapper = shallow(<RequestingProviderSelectedItem {...props} />);
        expect(wrapper).toBeTruthy();
    });
});
