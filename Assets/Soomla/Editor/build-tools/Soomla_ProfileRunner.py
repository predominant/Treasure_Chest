#!/usr/bin/python

from mod_pbxproj import *
from os import path, listdir
from shutil import copytree
import sys
import xml.etree.ElementTree as ET
import plistlib

script_dir = os.path.dirname(sys.argv[0])
build_path = sys.argv[1]
meta_data = sys.argv[2]

print ("script_dir:{0}".format(script_dir))
print ("build_path:{0}".format(build_path))
print ("metadata:{0}".format(meta_data))

frameworks = [
              'System/Library/Frameworks/Security.framework'
              ]

google_frameworks = [
                     'System/Library/Frameworks/AddressBook.framework',
                     'System/Library/Frameworks/AssetsLibrary.framework',
                     'System/Library/Frameworks/Foundation.framework',
                     'System/Library/Frameworks/CoreLocation.framework',
                     'System/Library/Frameworks/CoreMotion.framework',
                     'System/Library/Frameworks/CoreGraphics.framework',
                     'System/Library/Frameworks/CoreText.framework',
                     'System/Library/Frameworks/MediaPlayer.framework',
                     'System/Library/Frameworks/SystemConfiguration.framework',
                     'System/Library/Frameworks/UIKit.framework'
                    ]

twitter_frameworks = [
                     'System/Library/Frameworks/Twitter.framework',
                     'System/Library/Frameworks/Social.framework',
                     'System/Library/Frameworks/Accounts.framework'
                    ]

weak_frameworks = [

]

using_google_sdk = False
google_bundle_id = ""
using_twitter_sdk = False
twitter_consumer_key = ""

social_platform_data = meta_data.split(';')
for social_platform in social_platform_data:
    parsed_social_platform = social_platform.split('^')
    if parsed_social_platform[0] == "twitter":
        using_twitter_sdk = True
        twitter_consumer_key = parsed_social_platform[1]
    elif parsed_social_platform[0] == "google":
        using_google_sdk = True
        google_bundle_id = parsed_social_platform[1]

print ("echo {0} {1} {2} {3}".format(using_google_sdk, google_bundle_id, using_twitter_sdk, twitter_consumer_key))

pbx_file_path = build_path + '/Unity-iPhone.xcodeproj/project.pbxproj'
pbx_object = XcodeProject.Load(pbx_file_path)

for framework in frameworks:
    pbx_object.add_file_if_doesnt_exist(framework, tree='SDKROOT')

if using_google_sdk:
    for framework in google_frameworks:
        pbx_object.add_file_if_doesnt_exist(framework, tree='SDKROOT')
    # hopefully build_tools/../../../[Soomla]/Assets/Plugins/iOS
    google_framework_dir = path.join(script_dir,'..','..','..','WebPlayerTemplates','SoomlaConfig','ios', 'ios-profile-google', 'sdk')
    target_google_framework_dir = path.join(build_path, 'Libraries', 'ios-profile-google')
    copytree(google_framework_dir, target_google_framework_dir)
    pbx_object.add_framework_search_paths([path.abspath(target_google_framework_dir)])
    pbx_object.add_header_search_paths([path.abspath(target_google_framework_dir)])
    google_framework = path.join(target_google_framework_dir, 'GooglePlus.framework')
    pbx_object.add_file_if_doesnt_exist(path.abspath(google_framework), tree='SDKROOT')
    google_framework_open = path.join(target_google_framework_dir, 'GoogleOpenSource.framework')
    pbx_object.add_file_if_doesnt_exist(path.abspath(google_framework_open), tree='SDKROOT')
    google_resource_bundle = path.join(target_google_framework_dir, 'GooglePlus.bundle')
    pbx_object.add_file_if_doesnt_exist(path.abspath(google_resource_bundle))

if using_twitter_sdk:
    for framework in twitter_frameworks:
        pbx_object.add_file_if_doesnt_exist(framework, tree='SDKROOT')

for framework in weak_frameworks:
    pbx_object.add_file_if_doesnt_exist(framework, tree='SDKROOT', weak=True)

pbx_object.add_other_ldflags('-ObjC')

pbx_object.save()

plist_data = plistlib.readPlist(os.path.join(build_path, 'Info.plist'))
plist_types_arr = plist_data.get("CFBundleURLTypes")
if plist_types_arr == None:
    plist_types_arr = []
    plist_data["CFBundleURLTypes"] = plist_types_arr

if using_twitter_sdk:
    twitter_schemes = { "CFBundleURLSchemes" : ["tw{0}".format(twitter_consumer_key)]}
    plist_types_arr.append(twitter_schemes);

if using_google_sdk:
    google_schemes = { "CFBundleURLSchemes" : [google_bundle_id], "CFBundleURLName" : google_bundle_id }
    plist_types_arr.append(google_schemes);

plistlib.writePlist(plist_data, os.path.join(build_path, 'Info.plist'))
