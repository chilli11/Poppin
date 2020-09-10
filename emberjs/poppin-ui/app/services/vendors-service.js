import Service, { inject as injectService } from '@ember/service';
import { tracked } from '@glimmer/tracking';
import HttpResources from '../utils/http-resources';
import { Promise } from 'rsvp';

export default class VendorsService extends Service {
	namespace = 'VendorsService';
	@injectService apiService;

	@tracked vendors;

	createNewVendor(vendor) {
		return this.apiService.request({
			resource: HttpResources.createNewVendor,
			body: vendor
		});
	}

	updateVendor(vendor) {
		return this.apiService.request({
			resource: HttpResources.updateVendor,
			body: vendor
		});
	}

	getAllVendors() {
		return this.apiService.request({
			resource: HttpResources.getAllVendors
		}).then(vendors => this.vendors = vendors);
	}

	getVendorById(vId) {
		return this.apiService.request({
			resource: HttpResources.getVendorById,
			body: { vId }
		});
	}

	/**
	 * Gets a list of Vendors,
	 * based on a list of Ids
	 * Usually will be SubVendor list
	 * @param {Object} searchParams 
	 */
	getVendorsByList(list) {
		try {
			return this.apiService.request({
				resource: HttpResources.getVendorsByList,
				body: list
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}

	/**
	 * Gets a list of Vendors,
	 * based on a text search
	 * @param {Object} searchParams 
	 */
	getVendorsBySearch(searchTerm) {
		try {
			return this.apiService.request({
				resource: HttpResources.getVendorsBySearch,
				body: searchTerm
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}

	addMember({ vendorId, email, role, userId }) {
		try {
			return this.apiService.request({
				resource: HttpResources.addMember,
				body: { vendorId, email, role, userId }
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}

	removeMember({ vendorId, email, role, userId }) {
		try {
			return this.apiService.request({
				resource: HttpResources.removeMember,
				body: { vendorId, email, role, userId }
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}
}
