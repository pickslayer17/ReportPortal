import React from 'react';
import Navbar from './Navbar';
import { Outlet } from 'react-router-dom';

const Layout: React.FC = () => {
    return (
        <div>
            <Navbar />
            <div style={{ paddingTop: `calc(var(--navbar-height) + 2.5rem)` }}>
                <Outlet />
            </div>
        </div>
    );
};

export default Layout;