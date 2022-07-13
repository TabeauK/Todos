import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import FetchData from './components/Home';
import './custom.css'

export default () => (
    <Layout>
        <Route exact path='/' component={FetchData} />
        <Route exact path='/:userId' component={FetchData} />
    </Layout>
);
